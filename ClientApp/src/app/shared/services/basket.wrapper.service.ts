import { Injectable }       from '@angular/core';
import { Subject }          from 'rxjs';

import { ICatalogItem }     from '../models/catalogItem.model';
import { IBasketItem }      from '../models/basketItem.model';
import { IBasket }          from '../models/basket.model';
import { SecurityService } from '../services/security.service';
import { Guid } from '../models/guid';


@Injectable()
export class BasketWrapperService {
    public basket: IBasket;

    constructor() { }

    // observable that is fired when a product is added to the cart
    private addItemToBasketSource = new Subject<IBasketItem>();
    addItemToBasket$ = this.addItemToBasketSource.asObservable();

    private orderCreatedSource = new Subject();
    orderCreated$ = this.orderCreatedSource.asObservable();

    addItemToBasket(item: ICatalogItem) {
        
            let basketItem: IBasketItem = {
                pictureUrl: item.pictureUri,
                productId: item.id,
                productName: item.name,
                quantity: 1,
                unitPrice: item.price,
                id: Guid.newGuid(),
                oldUnitPrice: 0
            };

            this.addItemToBasketSource.next(basketItem);
    }

    orderCreated() {
        this.orderCreatedSource.next();
    }
}
