import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './shared/components/header/header';
import { AuthService } from './core/services/auth.service';
import { CartService } from './core/services/cart.service';
import { WishlistService } from './core/services/wishlist.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private auth = inject(AuthService);
  private cart = inject(CartService);
  private wishlist = inject(WishlistService);

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      const userId = this.auth.currentUserId()!;
      this.cart.loadCart(userId).subscribe();
      this.wishlist.loadWishlist(userId).subscribe();
    }
  }
}
