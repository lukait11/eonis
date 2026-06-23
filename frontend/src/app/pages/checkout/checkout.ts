import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';
import { AddressService } from '../../core/services/address.service';
import { OrderService } from '../../core/services/order.service';
import { PaymentService } from '../../core/services/payment.service';
import { Product, primaryImage } from '../../core/models/product.model';
import { CartItem } from '../../core/models/cart.model';
import { Address } from '../../core/models/address.model';

interface CartItemView {
  item: CartItem;
  product: Product | null;
}

type Phase = 'address' | 'payment' | 'success';

@Component({
  selector: 'app-checkout',
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css',
})
export class Checkout implements OnInit {
  private auth = inject(AuthService);
  private cartService = inject(CartService);
  private productService = inject(ProductService);
  private addressService = inject(AddressService);
  private orderService = inject(OrderService);
  private paymentService = inject(PaymentService);
  private fb = inject(FormBuilder);

  phase = signal<Phase>('address');
  loading = signal(true);
  processing = signal(false);
  errorMessage = signal<string | null>(null);

  itemViews = signal<CartItemView[]>([]);
  addresses = signal<Address[]>([]);
  selectedAddressId = signal<string | null>(null);
  showAddressForm = signal(false);
  orderId = signal<string | null>(null);

  total = computed(() =>
    this.itemViews().reduce((sum, v) => {
      if (!v.product) return sum;
      return sum + v.product.basePrice * (1 - v.product.discount / 100) * v.item.quantity;
    }, 0)
  );

  addressForm = this.fb.group({
    country: ['', Validators.required],
    city: ['', Validators.required],
    street: ['', Validators.required],
    postalCode: [null as number | null, Validators.required],
  });

  private _stripe: Stripe | null = null;
  private _elements: StripeElements | null = null;
  private _paymentElement: StripePaymentElement | null = null;

  ngOnInit(): void {
    const userId = this.auth.currentUserId()!;

    this.addressService.getByUser(userId).subscribe(addresses => {
      this.addresses.set(addresses);
      if (addresses.length > 0) this.selectedAddressId.set(addresses[0].id);
    });

    const cart = this.cartService.cart();
    if (!cart) { this.loading.set(false); return; }

    this.cartService.getItems(cart.id).subscribe(items => {
      if (items.length === 0) { this.loading.set(false); return; }
      let loaded = 0;
      const views: CartItemView[] = items.map(i => ({ item: i, product: null }));

      items.forEach((item, idx) => {
        if (!item.productVariant?.productId) {
          if (++loaded === items.length) { this.itemViews.set(views); this.loading.set(false); }
          return;
        }
        this.productService.getById(item.productVariant.productId).subscribe({
          next: p => { views[idx].product = p; },
          complete: () => {
            if (++loaded === items.length) { this.itemViews.set(views); this.loading.set(false); }
          },
        });
      });
    });
  }

  selectAddress(id: string): void {
    this.selectedAddressId.set(id);
  }

  saveAddress(): void {
    if (this.addressForm.invalid) return;
    const userId = this.auth.currentUserId()!;
    const val = this.addressForm.getRawValue();
    this.addressService.create({ ...val as any, userId }).subscribe(address => {
      this.addresses.update(a => [...a, address]);
      this.selectedAddressId.set(address.id);
      this.showAddressForm.set(false);
      this.addressForm.reset();
    });
  }

  async continueToPayment(): Promise<void> {
    const addressId = this.selectedAddressId();
    if (!addressId) return;
    this.processing.set(true);
    this.errorMessage.set(null);

    try {
      const userId = this.auth.currentUserId()!;
      const views = this.itemViews();
      const sellerProfileId = views.find(v => v.product)?.product?.sellerId;
      if (!sellerProfileId) throw new Error('Could not determine seller.');

      const baseAmount = views.reduce((s, v) => s + (v.product?.basePrice ?? 0) * v.item.quantity, 0);
      const discount   = views.reduce((s, v) => s + (v.product?.basePrice ?? 0) * ((v.product?.discount ?? 0) / 100) * v.item.quantity, 0);

      const order = await firstValueFrom(this.orderService.create({
        userId,
        sellerProfileId,
        addressId,
        baseAmount,
        discount,
        status: 'Pending',
      }));
      this.orderId.set(order.id);

      for (const view of views) {
        await firstValueFrom(this.orderService.createItem({
          orderId: order.id,
          productVariantId: view.item.productVariantId,
          quantity: view.item.quantity,
        }));
      }

      const intent = await firstValueFrom(this.paymentService.createIntent(order.id));

      const stripe = await loadStripe(environment.stripePublishableKey);
      if (!stripe) throw new Error('Stripe failed to load.');
      this._stripe = stripe;
      this._elements = stripe.elements({ clientSecret: intent.clientSecret });

      this.phase.set('payment');
      this.processing.set(false);

      setTimeout(() => {
        const el = this._elements!.create('payment');
        el.mount('#payment-element');
        this._paymentElement = el;
      }, 0);

    } catch (err: any) {
      this.errorMessage.set(err?.error?.message ?? err?.message ?? 'Something went wrong.');
      this.processing.set(false);
    }
  }

  async pay(): Promise<void> {
    if (!this._stripe || !this._elements) return;
    this.processing.set(true);
    this.errorMessage.set(null);

    const { error } = await this._stripe.confirmPayment({
      elements: this._elements,
      confirmParams: { return_url: `${window.location.origin}/checkout` },
      redirect: 'if_required',
    });

    if (error) {
      this.errorMessage.set(error.message ?? 'Payment failed.');
      this.processing.set(false);
    } else {
      this.phase.set('success');
      this.processing.set(false);
    }
  }

  getImage(product: Product | null): string | null {
    return product ? primaryImage(product) : null;
  }
}
