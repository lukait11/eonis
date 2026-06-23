import { Component, OnInit, signal, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { SellerProfileService } from '../../core/services/seller-profile.service';
import { OrderService } from '../../core/services/order.service';
import { AddressService } from '../../core/services/address.service';
import { ApplicationUser } from '../../core/models/user.model';
import { SellerProfile } from '../../core/models/seller-profile.model';
import { Order } from '../../core/models/order.model';
import { Address } from '../../core/models/address.model';

type Tab = 'profile' | 'orders' | 'addresses' | 'store';

@Component({
  selector: 'app-customer',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './customer.html',
  styleUrl: './customer.css',
})
export class Customer implements OnInit {
  auth = inject(AuthService);
  private userService = inject(UserService);
  private sellerService = inject(SellerProfileService);
  private orderService = inject(OrderService);
  private addressService = inject(AddressService);
  private fb = inject(FormBuilder);

  user = signal<ApplicationUser | null>(null);
  seller = signal<SellerProfile | null>(null);
  orders = signal<Order[]>([]);
  addresses = signal<Address[]>([]);
  selectedOrder = signal<Order | null>(null);
  loadingOrder = signal(false);
  loading = signal(true);
  activeTab = signal<Tab>('profile');

  saving = signal(false);
  saveMsg = signal('');
  uploadingAvatar = signal(false);

  profileForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phoneNumber: [''],
    dateOfBirth: [''],
  });

  storeForm = this.fb.group({
    storeName: ['', Validators.required],
    description: [''],
  });

  addressForm = this.fb.group({
    country: ['', Validators.required],
    city: ['', Validators.required],
    street: ['', Validators.required],
    postalCode: [0, Validators.required],
  });

  showAddressForm = signal(false);

  ngOnInit(): void {
    const userId = this.auth.currentUserId()!;
    this.userService.getUser(userId).subscribe({
      next: u => {
        this.user.set(u);
        this.profileForm.patchValue({
          firstName: u.firstName,
          lastName: u.lastName,
          phoneNumber: u.phoneNumber,
          dateOfBirth: u.dateOfBirth ? u.dateOfBirth.split('T')[0] : '',
        });
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });

    if (this.auth.currentUserRole() === 'Seller') {
      this.sellerService.getAll().subscribe(all => {
        const s = all.find(p => p.userId === userId);
        if (s) {
          this.seller.set(s);
          this.storeForm.patchValue({ storeName: s.storeName, description: s.description });
          this.orderService.getBySeller(s.id).subscribe(o => this.orders.set(o));
        }
      });
    } else {
      this.orderService.getByUser(userId).subscribe(o => this.orders.set(o));
    }

    this.addressService.getByUser(userId).subscribe(a => this.addresses.set(a));
  }

  setTab(t: Tab): void { this.activeTab.set(t); this.selectedOrder.set(null); }

  selectOrder(order: Order): void {
    this.loadingOrder.set(true);
    this.orderService.getById(order.id).subscribe({
      next: full => { this.selectedOrder.set(full); this.loadingOrder.set(false); },
      error: () => { this.selectedOrder.set(order); this.loadingOrder.set(false); },
    });
  }

  closeOrder(): void { this.selectedOrder.set(null); }

  saveProfile(): void {
    if (this.profileForm.invalid || !this.user()) return;
    this.saving.set(true);
    const raw = this.profileForm.getRawValue();
    const updated: ApplicationUser = {
      ...this.user()!,
      firstName: raw.firstName!,
      lastName: raw.lastName!,
      phoneNumber: raw.phoneNumber,
      dateOfBirth: raw.dateOfBirth || null,
    };
    this.userService.updateUser(updated).subscribe({
      next: u => {
        this.user.set(u);
        this.saveMsg.set('Profile saved.');
        this.saving.set(false);
        setTimeout(() => this.saveMsg.set(''), 2500);
      },
      error: () => { this.saveMsg.set('Failed to save.'); this.saving.set(false); },
    });
  }

  saveStore(): void {
    if (this.storeForm.invalid || !this.seller()) return;
    this.saving.set(true);
    const updated: SellerProfile = { ...this.seller()!, ...this.storeForm.getRawValue() as any };
    this.sellerService.update(updated).subscribe({
      next: s => {
        this.seller.set(s);
        this.saveMsg.set('Store updated.');
        this.saving.set(false);
        setTimeout(() => this.saveMsg.set(''), 2500);
      },
      error: () => { this.saveMsg.set('Failed to save.'); this.saving.set(false); },
    });
  }

  onAvatarChange(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.uploadingAvatar.set(true);
    this.userService.uploadProfilePicture(file).subscribe({
      next: url => {
        this.user.update(u => u ? { ...u, profilePictureUrl: url } : u);
        this.uploadingAvatar.set(false);
      },
      error: () => this.uploadingAvatar.set(false),
    });
  }

  addAddress(): void {
    if (this.addressForm.invalid) return;
    const userId = this.auth.currentUserId()!;
    this.addressService.create({ ...this.addressForm.getRawValue() as any, userId }).subscribe({
      next: a => {
        this.addresses.update(list => [...list, a]);
        this.addressForm.reset();
        this.showAddressForm.set(false);
      },
    });
  }

  deleteAddress(id: string): void {
    this.addressService.delete(id).subscribe(() =>
      this.addresses.update(list => list.filter(a => a.id !== id))
    );
  }

  statusColor(status: string): string {
    const map: Record<string, string> = {
      Delivered: '#52c97a', Shipped: '#60aaff', Paid: '#c9a96e', Cancelled: '#f08080', Pending: '#909090',
    };
    return map[status] ?? '#909090';
  }
}
