import { Component, OnInit, signal, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { SellerProfileService } from '../../core/services/seller-profile.service';
import { ProductService } from '../../core/services/product.service';
import { CategoryService } from '../../core/services/category.service';
import { Product, ProductVariant, ProductImage, primaryImage, effectivePrice } from '../../core/models/product.model';
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

  expandedProductId = signal<string | null>(null);
  savingVariant = signal(false);
  editingVariantId = signal<string | null>(null);

  expandedImagesProductId = signal<string | null>(null);
  productImages = signal<Record<string, ProductImage[]>>({});
  loadingImagesId = signal<string | null>(null);

  form = this.fb.group({
    name: ['', Validators.required],
    description: [''],
    basePrice: [0, [Validators.required, Validators.min(0)]],
    discount: [0, [Validators.min(0), Validators.max(100)]],
    material: [''],
    status: ['Available'],
    categoryIds: [[] as string[]],
  });

  variantForm = this.fb.group({
    size: ['', Validators.required],
    color: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(0)]],
  });

  editVariantForm = this.fb.group({
    size: ['', Validators.required],
    color: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(0)]],
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
    this.form.reset({ status: 'Available', discount: 0, basePrice: 0, categoryIds: [] });
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
      categoryIds: (p.categories ?? []).map(c => c.id),
    });
    this.showForm.set(true);
  }

  cancelForm(): void { this.showForm.set(false); this.editProduct.set(null); }

  submit(): void {
    if (this.form.invalid || !this.seller()) return;
    this.saving.set(true);
    const val = this.form.getRawValue();
    const editing = this.editProduct();
    const selectedCategories = this.categories().filter(c => (val.categoryIds ?? []).includes(c.id));

    if (editing) {
      const updated: Product = { ...editing, ...val as any, categories: selectedCategories };
      this.productService.update(updated).subscribe({
        next: p => {
          this.products.update(list => list.map(x => x.id === p.id ? { ...p, variants: x.variants, images: x.images } : x));
          this.cancelForm();
          this.saving.set(false);
        },
        error: () => this.saving.set(false),
      });
    } else {
      this.productService.create({ ...val as any, sellerId: this.seller()!.id, categoryIds: val.categoryIds ?? [] }).subscribe({
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
          const existing = p.images ?? [];
          const imgs = [...existing, { id: crypto.randomUUID(), productId, imageUrl: url, isPrimary: existing.length === 0 }];
          return { ...p, images: imgs };
        }));
        this.uploadingId.set(null);
      },
      error: () => this.uploadingId.set(null),
    });
  }

  toggleVariants(productId: string): void {
    if (this.expandedProductId() === productId) {
      this.expandedProductId.set(null);
      this.editingVariantId.set(null);
    } else {
      this.expandedProductId.set(productId);
      this.variantForm.reset({ quantity: 1 });
      this.editingVariantId.set(null);
    }
  }

  openEditVariant(v: ProductVariant): void {
    this.editingVariantId.set(v.id);
    this.editVariantForm.setValue({ size: v.size, color: v.color, quantity: v.quantity });
  }

  cancelEditVariant(): void {
    this.editingVariantId.set(null);
  }

  saveVariant(v: ProductVariant, productId: string): void {
    if (this.editVariantForm.invalid) return;
    this.savingVariant.set(true);
    const val = this.editVariantForm.getRawValue();
    const updated: ProductVariant = { ...v, size: val.size!, color: val.color!, quantity: val.quantity! };
    this.productService.updateVariant(updated).subscribe({
      next: saved => {
        this.products.update(list => list.map(p =>
          p.id === productId
            ? { ...p, variants: (p.variants ?? []).map(x => x.id === saved.id ? saved : x) }
            : p
        ));
        this.editingVariantId.set(null);
        this.savingVariant.set(false);
      },
      error: () => this.savingVariant.set(false),
    });
  }

  addVariant(productId: string): void {
    if (this.variantForm.invalid) return;
    this.savingVariant.set(true);
    const val = this.variantForm.getRawValue();
    this.productService.createVariant({ ...val as any, productId }).subscribe({
      next: v => {
        this.products.update(list => list.map(p =>
          p.id === productId ? { ...p, variants: [...(p.variants ?? []), v] } : p
        ));
        this.variantForm.reset({ quantity: 1 });
        this.savingVariant.set(false);
      },
      error: () => this.savingVariant.set(false),
    });
  }

  deleteVariant(variantId: string, productId: string): void {
    this.productService.deleteVariant(variantId).subscribe(() => {
      this.products.update(list => list.map(p =>
        p.id === productId
          ? { ...p, variants: (p.variants ?? []).filter(v => v.id !== variantId) }
          : p
      ));
    });
  }

  toggleImages(productId: string): void {
    if (this.expandedImagesProductId() === productId) {
      this.expandedImagesProductId.set(null);
      return;
    }
    this.expandedImagesProductId.set(productId);
    if (this.productImages()[productId]) return;
    this.loadingImagesId.set(productId);
    this.productService.getImages(productId).subscribe({
      next: imgs => {
        this.productImages.update(m => ({ ...m, [productId]: imgs }));
        this.loadingImagesId.set(null);
      },
      error: () => this.loadingImagesId.set(null),
    });
  }

  setPrimaryImage(imageId: string, productId: string): void {
    this.productService.setPrimaryImage(productId, imageId).subscribe(() => {
      this.productImages.update(m => ({
        ...m,
        [productId]: (m[productId] ?? []).map(i => ({ ...i, isPrimary: i.id === imageId })),
      }));
      this.products.update(list => list.map(p => {
        if (p.id !== productId) return p;
        const all = this.productImages()[productId] ?? [];
        const newPrimary = all.find(i => i.id === imageId);
        return { ...p, images: newPrimary ? [newPrimary] : p.images };
      }));
    });
  }

  deleteImage(imageId: string, productId: string): void {
    this.productService.deleteImage(productId, imageId).subscribe(() => {
      const remaining = (this.productImages()[productId] ?? []).filter(i => i.id !== imageId);
      this.productImages.update(m => ({ ...m, [productId]: remaining }));
      this.products.update(list => list.map(p => {
        if (p.id !== productId) return p;
        const newPrimary = remaining.find(i => i.isPrimary) ?? remaining[0] ?? null;
        return { ...p, images: newPrimary ? [newPrimary] : [] };
      }));
    });
  }

  getImage = primaryImage;
  getPrice = effectivePrice;
}
