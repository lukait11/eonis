import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductCard } from '../../shared/components/product-card/product-card';
import { ProductService } from '../../core/services/product.service';
import { CategoryService } from '../../core/services/category.service';
import { Product } from '../../core/models/product.model';
import { Category } from '../../core/models/category.model';

type SortOption = 'default' | 'price-asc' | 'price-desc' | 'newest';

@Component({
  selector: 'app-explore',
  imports: [FormsModule, ProductCard],
  templateUrl: './explore.html',
  styleUrl: './explore.css',
})
export class Explore implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);

  items = signal<Product[]>([]);
  totalCount = signal(0);
  categories = signal<Category[]>([]);
  loading = signal(true);

  readonly pageSize = 12;
  page = signal(1);
  search = signal('');
  selectedCategory = signal('');
  sort = signal<SortOption>('default');

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize));

  pageNumbers = computed(() => {
    const total = this.totalPages();
    if (total <= 1) return [];
    const current = this.page();
    let start = Math.max(1, current - 2);
    let end = Math.min(total, start + 4);
    start = Math.max(1, end - 4);
    const pages: number[] = [];
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  });

  rangeStart = computed(() => (this.page() - 1) * this.pageSize + 1);
  rangeEnd = computed(() => Math.min(this.page() * this.pageSize, this.totalCount()));

  private _searchDebounce: ReturnType<typeof setTimeout> | undefined;

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => this.categories.set(cats));

    this.route.queryParams.subscribe(params => {
      if (params['category']) this.selectedCategory.set(params['category']);
      this.load();
    });
  }

  load(): void {
    this.loading.set(true);
    this.productService.getPaged(
      this.page(),
      this.pageSize,
      this.search() || undefined,
      this.selectedCategory() || undefined,
      this.sort(),
    ).subscribe({
      next: result => {
        this.items.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onSearch(value: string): void {
    this.search.set(value);
    clearTimeout(this._searchDebounce);
    this._searchDebounce = setTimeout(() => { this.page.set(1); this.load(); }, 300);
  }

  onCategory(value: string): void {
    this.selectedCategory.set(value);
    this.page.set(1);
    this.load();
  }

  onSort(value: string): void {
    this.sort.set(value as SortOption);
    this.page.set(1);
    this.load();
  }

  goTo(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.load();
  }

  clearFilters(): void {
    this.search.set('');
    this.selectedCategory.set('');
    this.sort.set('default');
    this.page.set(1);
    this.load();
  }
}
