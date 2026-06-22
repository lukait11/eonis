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

  all = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(true);

  search = signal('');
  selectedCategory = signal('');
  sort = signal<SortOption>('default');

  filtered = computed(() => {
    let list = [...this.all()];
    const q = this.search().toLowerCase().trim();
    const cat = this.selectedCategory();

    if (q) list = list.filter(p => p.name.toLowerCase().includes(q));
    if (cat) list = list.filter(p => p.categoryId === cat);

    switch (this.sort()) {
      case 'price-asc':
        list.sort((a, b) => a.basePrice * (1 - a.discount / 100) - b.basePrice * (1 - b.discount / 100));
        break;
      case 'price-desc':
        list.sort((a, b) => b.basePrice * (1 - b.discount / 100) - a.basePrice * (1 - a.discount / 100));
        break;
      case 'newest':
        list.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        break;
    }
    return list;
  });

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => this.categories.set(cats));

    this.route.queryParams.subscribe(params => {
      if (params['category']) this.selectedCategory.set(params['category']);
    });

    this.productService.getAll().subscribe({
      next: products => {
        this.all.set(products);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onSearch(value: string): void { this.search.set(value); }
  onCategory(value: string): void { this.selectedCategory.set(value); }
  onSort(value: string): void { this.sort.set(value as SortOption); }
  clearFilters(): void {
    this.search.set('');
    this.selectedCategory.set('');
    this.sort.set('default');
  }
}
