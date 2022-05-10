import { Inject, Injectable } from '@angular/core';

import { DataService } from '../shared/services/data.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { ICatalog } from '../shared/models/catalog.model';
import { ICatalogBrand } from '../shared/models/catalogBrand.model';
import { ICatalogType } from '../shared/models/catalogType.model';

import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class CatalogService {
    private catalogUrl: string = '';
    private brandUrl: string = '';
    private typesUrl: string = '';
  
  constructor(private service: DataService, @Inject('BASE_URL') baseUrl: string) {
        
    this.catalogUrl = baseUrl + 'catalog/items';
    this.brandUrl = baseUrl+ 'catalog/catalogbrands';
    this.typesUrl = baseUrl + 'catalog/catalogtypes';
        
    }

    getCatalog(pageIndex: number, pageSize: number, brand: number, type: number): Observable<ICatalog> {
        let url = this.catalogUrl;

        if (type) {
            url = this.catalogUrl + '/type/' + type.toString() + '/brand/' + ((brand) ? brand.toString() : '');
        }
        else if (brand) {
            url = this.catalogUrl + '/type/all' + '/brand/' + ((brand) ? brand.toString() : '');
        }
      
        url = url + '?pageIndex=' + pageIndex + '&pageSize=' + pageSize;

        return this.service.get(url).pipe<ICatalog>(tap((response: any) => {
            return response;
        }));
    }

    getBrands(): Observable<ICatalogBrand[]> {
        return this.service.get(this.brandUrl).pipe<ICatalogBrand[]>(tap((response: any) => {
            return response;
        }));
    }

    getTypes(): Observable<ICatalogType[]> {
        return this.service.get(this.typesUrl).pipe<ICatalogType[]>(tap((response: any) => {
            return response;
        }));
    };
}
