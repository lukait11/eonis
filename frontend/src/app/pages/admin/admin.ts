import { Component, OnInit, signal, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { CategoryService } from '../../core/services/category.service';
import { OrderService } from '../../core/services/order.service';
import { ApplicationUser } from '../../core/models/user.model';
import { Category } from '../../core/models/category.model';
import { Order, OrderStatus } from '../../core/models/order.model';

type AdminTab = 'categories' | 'users' | 'sellers' | 'orders';

@Component({
  selector: 'app-admin',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin implements OnInit {
  private userService = inject(UserService);
  private categoryService = inject(CategoryService);
  private orderService = inject(OrderService);
  private fb = inject(FormBuilder);

  activeTab = signal<AdminTab>('categories');
  loading = signal(true);

  // Categories
  categories = signal<Category[]>([]);
  showCategoryForm = signal(false);
  editingCategory = signal<Category | null>(null);
  savingCategory = signal(false);
  deletingCategoryId = signal<string | null>(null);

  categoryForm = this.fb.group({
    name: ['', Validators.required],
    description: [''],
  });

  // Users (customers)
  users = signal<ApplicationUser[]>([]);
  editingUser = signal<ApplicationUser | null>(null);
  savingUser = signal(false);
  deleteConfirmUserId = signal<string | null>(null);

  userForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phoneNumber: [''],
    dateOfBirth: [''],
  });

  // Sellers
  sellers = signal<ApplicationUser[]>([]);
  editingSeller = signal<ApplicationUser | null>(null);
  savingSeller = signal(false);
  deleteConfirmSellerId = signal<string | null>(null);

  sellerForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phoneNumber: [''],
    dateOfBirth: [''],
  });

  // Orders
  orders = signal<Order[]>([]);
  updatingOrderId = signal<string | null>(null);

  readonly orderStatuses: OrderStatus[] = ['Pending', 'Paid', 'Shipped', 'Delivered', 'Cancelled'];

  ngOnInit(): void {
    this.loadAll();
  }

  private loadAll(): void {
    this.loading.set(true);
    let remaining = 4;
    const done = () => { if (--remaining === 0) this.loading.set(false); };

    this.categoryService.getAll().subscribe({ next: c => { this.categories.set(c); done(); }, error: done });
    this.orderService.getAll().subscribe({ next: o => { this.orders.set(o); done(); }, error: done });
    this.userService.getAll().subscribe({
      next: all => {
        this.users.set(all.filter(u => u.role === 'Customer'));
        this.sellers.set(all.filter(u => u.role === 'Seller'));
        done();
        done(); // counts for both users + sellers
      },
      error: () => { done(); done(); },
    });
  }

  setTab(tab: AdminTab): void {
    this.activeTab.set(tab);
    this.closeAllModals();
  }

  private closeAllModals(): void {
    this.showCategoryForm.set(false);
    this.editingCategory.set(null);
    this.editingUser.set(null);
    this.editingSeller.set(null);
    this.deleteConfirmUserId.set(null);
    this.deleteConfirmSellerId.set(null);
    this.deletingCategoryId.set(null);
  }

  // ── Categories ──────────────────────────────────────────────────────────────

  openCreateCategory(): void {
    this.editingCategory.set(null);
    this.categoryForm.reset();
    this.showCategoryForm.set(true);
  }

  openEditCategory(c: Category): void {
    this.editingCategory.set(c);
    this.categoryForm.patchValue({ name: c.name, description: c.description });
    this.showCategoryForm.set(true);
  }

  closeCategoryForm(): void {
    this.showCategoryForm.set(false);
    this.editingCategory.set(null);
  }

  saveCategory(): void {
    if (this.categoryForm.invalid) return;
    this.savingCategory.set(true);
    const val = this.categoryForm.getRawValue();
    const editing = this.editingCategory();

    const req = { name: val.name!, description: val.description ?? '' };

    if (editing) {
      this.categoryService.update(editing.id, req).subscribe({
        next: updated => {
          this.categories.update(list => list.map(c => c.id === updated.id ? updated : c));
          this.closeCategoryForm();
          this.savingCategory.set(false);
        },
        error: () => this.savingCategory.set(false),
      });
    } else {
      this.categoryService.create(req).subscribe({
        next: created => {
          this.categories.update(list => [...list, created]);
          this.closeCategoryForm();
          this.savingCategory.set(false);
        },
        error: () => this.savingCategory.set(false),
      });
    }
  }

  confirmDeleteCategory(id: string): void { this.deletingCategoryId.set(id); }
  cancelDeleteCategory(): void { this.deletingCategoryId.set(null); }

  deleteCategory(id: string): void {
    this.categoryService.delete(id).subscribe({
      next: () => {
        this.categories.update(list => list.filter(c => c.id !== id));
        this.deletingCategoryId.set(null);
      },
    });
  }

  // ── Users ───────────────────────────────────────────────────────────────────

  openEditUser(u: ApplicationUser): void {
    this.editingUser.set(u);
    this.userForm.patchValue({
      firstName: u.firstName,
      lastName: u.lastName,
      phoneNumber: u.phoneNumber,
      dateOfBirth: u.dateOfBirth ? u.dateOfBirth.split('T')[0] : '',
    });
  }

  closeEditUser(): void { this.editingUser.set(null); }

  saveUser(): void {
    if (this.userForm.invalid || !this.editingUser()) return;
    this.savingUser.set(true);
    const val = this.userForm.getRawValue();
    const updated: ApplicationUser = {
      ...this.editingUser()!,
      firstName: val.firstName!,
      lastName: val.lastName!,
      phoneNumber: val.phoneNumber,
      dateOfBirth: val.dateOfBirth || null,
    };
    this.userService.updateUser(updated).subscribe({
      next: saved => {
        this.users.update(list => list.map(u => u.id === saved.id ? saved : u));
        this.editingUser.set(null);
        this.savingUser.set(false);
      },
      error: () => this.savingUser.set(false),
    });
  }

  confirmDeleteUser(id: string): void { this.deleteConfirmUserId.set(id); }
  cancelDeleteUser(): void { this.deleteConfirmUserId.set(null); }

  deleteUser(id: string): void {
    this.userService.deleteUser(id).subscribe({
      next: () => {
        this.users.update(list => list.filter(u => u.id !== id));
        this.deleteConfirmUserId.set(null);
      },
    });
  }

  // ── Sellers ─────────────────────────────────────────────────────────────────

  openEditSeller(u: ApplicationUser): void {
    this.editingSeller.set(u);
    this.sellerForm.patchValue({
      firstName: u.firstName,
      lastName: u.lastName,
      phoneNumber: u.phoneNumber,
      dateOfBirth: u.dateOfBirth ? u.dateOfBirth.split('T')[0] : '',
    });
  }

  closeEditSeller(): void { this.editingSeller.set(null); }

  saveSeller(): void {
    if (this.sellerForm.invalid || !this.editingSeller()) return;
    this.savingSeller.set(true);
    const val = this.sellerForm.getRawValue();
    const updated: ApplicationUser = {
      ...this.editingSeller()!,
      firstName: val.firstName!,
      lastName: val.lastName!,
      phoneNumber: val.phoneNumber,
      dateOfBirth: val.dateOfBirth || null,
    };
    this.userService.updateUser(updated).subscribe({
      next: saved => {
        this.sellers.update(list => list.map(u => u.id === saved.id ? saved : u));
        this.editingSeller.set(null);
        this.savingSeller.set(false);
      },
      error: () => this.savingSeller.set(false),
    });
  }

  confirmDeleteSeller(id: string): void { this.deleteConfirmSellerId.set(id); }
  cancelDeleteSeller(): void { this.deleteConfirmSellerId.set(null); }

  deleteSeller(id: string): void {
    this.userService.deleteUser(id).subscribe({
      next: () => {
        this.sellers.update(list => list.filter(u => u.id !== id));
        this.deleteConfirmSellerId.set(null);
      },
    });
  }

  // ── Orders ──────────────────────────────────────────────────────────────────

  changeOrderStatus(order: Order, status: string): void {
    this.updatingOrderId.set(order.id);
    this.orderService.updateStatus(order.id, status).subscribe({
      next: updated => {
        this.orders.update(list => list.map(o => o.id === updated.id ? updated : o));
        this.updatingOrderId.set(null);
      },
      error: () => this.updatingOrderId.set(null),
    });
  }

  statusColor(status: string): string {
    const map: Record<string, string> = {
      Delivered: '#52c97a', Shipped: '#60aaff', Paid: '#c9a96e',
      Cancelled: '#f08080', Pending: '#909090',
    };
    return map[status] ?? '#909090';
  }
}
