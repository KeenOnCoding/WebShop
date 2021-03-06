import { Inject, Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';

import { DataService } from '../shared/services/data.service';
import { SecurityService } from '../shared/services/security.service';
import { IBasket } from '../shared/models/basket.model';
import { IOrder } from '../shared/models/order.model';
import { IBasketCheckout } from '../shared/models/basketCheckout.model';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { StorageService } from '../shared/services/storage.service';

import { Observable, Observer, Subject } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';

@Injectable()
export class BasketService {
  private basketUrl: string = '';
  private purchaseUrl: string = '';
    basket: IBasket = {
      //buyerId: '228CB100-C3E5-404C-82BE-16CC076733DC',
      buyerId: '32c0cc83-ee3d-4b9a-8f21-fddcb1a15ea4',
        items: []
    };

    //observable that is fired when item is removed from basket
    private basketUpdateSource = new Subject();
    basketUpdate$ = this.basketUpdateSource.asObservable();

  constructor(private service: DataService,
    private basketWrapperService: BasketWrapperService,
    @Inject('BASE_URL') baseUrl: string) {
    this.basket.items = [];
    this.loadData();
    this.basketUrl = baseUrl;
    this.purchaseUrl = baseUrl;
  }
        


    

    addItemToBasket(item): Observable<boolean> {
        let basketItem = this.basket.items.find(value => value.productId == item.productId);

        if (basketItem) {
            basketItem.quantity++;
        } else {
            this.basket.items.push(item);
        }

        return this.setBasket(this.basket);
    }

    setBasket(basket): Observable<boolean> {
        let url = this.purchaseUrl + 'cart';

        this.basket = basket;

        return this.service.post(url, basket).pipe<boolean>(tap((response: any) => true));
    }

    setBasketCheckout(basketCheckout): Observable<boolean> {
      let url = this.basketUrl + 'cart/checkout';

        return this.service.postWithId(url, basketCheckout).pipe<boolean>(tap((response: any) => {
            this.basketWrapperService.orderCreated();
            return true;
        }));
    }

    getBasket(): Observable<IBasket> {
      let url = this.basketUrl + 'cart/' + this.basket.buyerId;

        return this.service.get(url).pipe<IBasket>(tap((response: any) => {
            if (response.status === 204) {
                return null;
            }

            return response;
        }));
    }

    mapBasketInfoCheckout(order: IOrder): IBasketCheckout {
        let basketCheckout = <IBasketCheckout>{};

        basketCheckout.street = order.street
        basketCheckout.city = order.city;
        basketCheckout.country = order.country;
        basketCheckout.state = order.state;
        basketCheckout.zipcode = order.zipcode;
        basketCheckout.cardexpiration = order.cardexpiration;
        basketCheckout.cardnumber = order.cardnumber;
        basketCheckout.cardsecuritynumber = order.cardsecuritynumber;
        basketCheckout.cardtypeid = order.cardtypeid;
      basketCheckout.cardholdername = order.cardholdername;
      basketCheckout.buyer = order.buyer;
        basketCheckout.total = 0;
      basketCheckout.expiration = order.expiration;
      basketCheckout.orderItems = order.orderItems;
        return basketCheckout;
    }

    updateQuantity() {
        this.basketUpdateSource.next();
    }

    dropBasket() {
        this.basket.items = [];
        this.setBasket(this.basket).subscribe(res => {
            this.basketUpdateSource.next();
        });
    }

    private loadData() {
        this.getBasket().subscribe(basket => {
            if (basket != null)
                this.basket.items = basket.items;
        });
    }
}
