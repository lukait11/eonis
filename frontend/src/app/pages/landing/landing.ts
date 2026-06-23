import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductCard } from '../../shared/components/product-card/product-card';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, ProductCard],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing implements OnInit {
  private productService = inject(ProductService);

  featured = signal<Product[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.productService.getAll().subscribe({
      next: products => {
        this.featured.set(products.slice(0, 8));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
