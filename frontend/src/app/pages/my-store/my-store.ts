import { Component, OnInit, signal, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { SellerProfileService } from '../../core/services/seller-profile.service';
import { ProductService } from '../../core/services/product.service';
import { CategoryService } from '../../core/services/category.service';
import { Product, primaryImage, effectivePrice } from '../../core/models/product.model';
import { Category } from '../../core/models/category.model';
import { SellerProfile } from '../../core/models/seller-profile.model';

@Component({
  selector: 'app-my-store',
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './my-store.html',
  styleUrl: './my-store.css',
})
export class MyStore implements OnInit {
  auth = inject(AuthService);
  private sellerService = inject(SellerProfileService);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  seller = signal<SellerProfile | null>(null);
  products = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(true);
  showForm = signal(false);
  editProduct = signal<Product | null>(null);
  saving = signal(false);
  uploadingId = signal<string | null>(null);
  deleteConfirm = signal<string | null>(null);

  form = this.fb.group({
    name: ['', Validators.required],
    description: [''],
    basePrice: [0, [Validators.required, Validators.min(0)]],
    discount: [0, [Validators.min(0), Validators.max(100)]],
    material: [''],
    status: ['Available'],
    categoryId: [null as string | null],
  });

  ngOnInit(): void {
    const userId = this.auth.currentUserId()!;
    this.categoryService.getAll().subscribe(c => this.categories.set(c));
    this.sellerService.getAll().subscribe(all => {
      const s = all.find(p => p.userId === userId);
      if (s) {
        this.seller.set(s);
        this.productService.getAll().subscribe(all => {
          this.products.set(all.filter(p => p.sellerId === s.id));
          this.loading.set(false);
        });
      } else {
        this.loading.set(false);
      }
    });
  }

  openCreate(): void {
    this.editProduct.set(null);
    this.form.reset({ status: 'Available', discount: 0, basePrice: 0, categoryId: null });
    this.showForm.set(true);
  }

  openEdit(p: Product): void {
    this.editProduct.set(p);
    this.form.patchValue({
      name: p.name,
      description: p.description,
      basePrice: p.basePrice,
      discount: p.discount,
      material: p.material,
      status: p.status,
      categoryId: p.categoryId,
    });
    this.showForm.set(true);
  }

  cancelForm(): void { this.showForm.set(false); this.editProduct.set(null); }

  submit(): void {
    if (this.form.invalid || !this.seller()) return;
    this.saving.set(true);
    const val = this.form.getRawValue();
    const editing = this.editProduct();

    if (editing) {
      const updated: Product = { ...editing, ...val as any };
      this.productService.update(updated).subscribe({
        next: p => {
          this.products.update(list => list.map(x => x.id === p.id ? p : x));
          this.cancelForm();
          this.saving.set(false);
        },
        error: () => this.saving.set(false),
      });
    } else {
      this.productService.create({ ...val as any, sellerId: this.seller()!.id }).subscribe({
        next: p => {
          this.products.update(list => [...list, p]);
          this.cancelForm();
          this.saving.set(false);
        },
        error: () => this.saving.set(false),
      });
    }
  }

  confirmDelete(id: string): void { this.deleteConfirm.set(id); }
  cancelDelete(): void { this.deleteConfirm.set(null); }

  deleteProduct(id: string): void {
    this.productService.delete(id).subscribe(() => {
      this.products.update(list => list.filter(p => p.id !== id));
      this.deleteConfirm.set(null);
    });
  }

  uploadImage(productId: string, event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.uploadingId.set(productId);
    this.productService.uploadImage(productId, file).subscribe({
      next: url => {
        this.products.update(list => list.map(p => {
          if (p.id !== productId) return p;
          const imgs = [...(p.images ?? []), { id: crypto.randomUUID(), productId, imageUrl: url, isPrimary: false }];
          return { ...p, images: imgs };
        }));
        this.uploadingId.set(null);
      },
      error: () => this.uploadingId.set(null),
    });
  }

  getImage = primaryImage;
  getPrice = effectivePrice;
}
