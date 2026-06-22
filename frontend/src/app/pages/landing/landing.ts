import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductCard } from '../../shared/components/product-card/product-card';
import { ProductService } from '../../core/services/product.service';
import { CategoryService } from '../../core/services/category.service';
import { Product } from '../../core/models/product.model';
import { Category } from '../../core/models/category.model';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, ProductCard],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);

  featured = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.productService.getAll().subscribe({
      next: products => {
        this.featured.set(products.slice(0, 8));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });

    this.categoryService.getAll().subscribe({
      next: cats => this.categories.set(cats.slice(0, 6)),
    });
  }
}
