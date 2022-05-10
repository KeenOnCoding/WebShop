import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { CatalogComponent } from './catalog/catalog.component';
import { ToastrModule } from 'ngx-toastr';
import { SharedModule } from './shared/shared.module';
import { BasketComponent } from './basket/basket.component';
import { BasketModule } from './basket/basket.module';
import { CatalogModule } from './catalog/catalog.module';
import { OrdersComponent } from './orders/orders.component';
import { OrdersDetailComponent } from './orders/orders-detail/orders-detail.component';
import { OrdersNewComponent } from './orders/orders-new/orders-new.component';
import { OrdersModule } from './orders/orders.module';

import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    BrowserModule,
    CatalogModule,
    BasketModule,
    OrdersModule,
    ToastrModule.forRoot(),
    HttpClientModule,
    ApiAuthorizationModule,
    // Only module that app module loads
    SharedModule.forRoot(),
    RouterModule.forRoot([
      /*{ path: '', component: HomeComponent, pathMatch: 'full' },*/
      { path: '', redirectTo: 'catalog', pathMatch: 'full' },
      { path: 'catalog/basket', redirectTo: 'basket', pathMatch: 'full' },
      { path: 'basket', component: BasketComponent },
      { path: 'catalog', component: CatalogComponent },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthorizeGuard] },
      { path: 'orders', component: OrdersComponent },
      { path: 'orders/:id', component: OrdersDetailComponent },
      { path: 'order', component: OrdersNewComponent, canActivate: [AuthorizeGuard]},
    ])
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
