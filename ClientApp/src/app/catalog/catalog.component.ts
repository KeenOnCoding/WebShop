import { Component, OnInit }    from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { catchError, map, startWith }           from 'rxjs/operators';
import { FormControl } from '@angular/forms';

import { CatalogService }       from './catalog.service';
//import { ConfigurationService } from '../shared/services/configuration.service';
import { ICatalog }             from '../shared/models/catalog.model';
import { ICatalogItem }         from '../shared/models/catalogItem.model';
import { ICatalogType }         from '../shared/models/catalogType.model';
import { ICatalogBrand }        from '../shared/models/catalogBrand.model';
import { IPager }               from '../shared/models/pager.model';
import { BasketWrapperService}  from '../shared/services/basket.wrapper.service';
//import { SecurityService }      from '../shared/services/security.service';

@Component({
    selector: 'esh-catalog .esh-catalog .mb-5',
    styleUrls: ['./catalog.component.scss'],
    templateUrl: './catalog.component.html'
})
export class CatalogComponent implements OnInit {

  title = "Serch";
  keyword = "name";
  public countries = [
    ".NET",
    "T-Shirt",
    "Visual Studio",
    "All",
    "Azure",
    "SQL Server",
    "USB Memory Stick",
    "Sheet",
    "Mug"
  ];

  brands: ICatalogBrand[];
  types: ICatalogType[];
  catalog: ICatalog;
  brandSelected: number;
  typeSelected: number;
  paginationInfo: IPager;
  authenticated: boolean = false;
  //authSubscription: Subscription;
  errorReceived: boolean;
  searchItem: string = "";

  myControl = new FormControl();
  options: string[] = ['One', 'Two', 'Three', 'Four', 'Five', 'Six'];
  filteredOptions: Observable<string[]>;

  constructor(private service: CatalogService, private basketService: BasketWrapperService) {
  }

  ngOnInit() {
    this.loadData();
    this.filteredOptions = this.myControl.valueChanges.pipe(
      startWith(''),
      map(value => this._filter(value)),
    );
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.options.filter(option => option.toLowerCase().includes(filterValue));
  }

  loadData() {
    this.getBrands();
    this.getCatalog(12, 0);
    this.getTypes();
  }
  selectEvent(item) {
    // do something with selected item
    let searchBrand = this.brands;
    let searchType = this.types;

    let result1: ICatalogBrand[];
    let result2: ICatalogType[];

    let brandSelected: number;
    let typeSelected: number;

    result1 = searchBrand.filter(x => x.brand === item);
    result2 = searchType.filter(x => x.type === item);
    if (result1.length != 0) {
      brandSelected = result1[0].id;
    }
    if (result2.length != 0) {
      typeSelected = result2[0].id;
    }

    brandSelected = brandSelected && brandSelected.toString() != "null" ? brandSelected : 0;
    typeSelected = typeSelected && typeSelected.toString() != "null" ? typeSelected : 0;
    this.paginationInfo.actualPage = 0;

    this.getCatalog(this.paginationInfo.itemsPage, this.paginationInfo.actualPage, brandSelected, typeSelected);
  }
  onFilterSearch() {
    //let arrSearch = [
    //  {
    //    groupname: 'brands',
    //    members: [] = this.brands
    //  },
    //  {
    //    groupname: 'types',
    //    members: [] = this.types
    //  }
    //];

    //let result = arrSearch.filter(x => x.members.some((a) => a.type === value));

    let searchBrand = this.brands;
    let searchType = this.types;

    let result1: ICatalogBrand[];
    let result2: ICatalogType[];

    let brandSelected: number;
    let typeSelected: number;

    result1 = searchBrand.filter(x => x.brand === this.searchItem);
    result2 = searchType.filter(x => x.type === this.searchItem);
    if (result1.length != 0) {
      brandSelected = result1[0].id;
    }
    if (result2.length != 0) {
      typeSelected = result2[0].id;
    }

    

    brandSelected = brandSelected && brandSelected.toString() != "null" ? brandSelected : 0;
    typeSelected = typeSelected && typeSelected.toString() != "null" ? typeSelected : 0;
    this.paginationInfo.actualPage = 0;

    this.getCatalog(this.paginationInfo.itemsPage, this.paginationInfo.actualPage, brandSelected, typeSelected);

  }

  onFilterApplied(event: any) {
    event.preventDefault();
    this.brandSelected = this.brandSelected && this.brandSelected.toString() != "null" ? this.brandSelected : null;
    this.typeSelected = this.typeSelected && this.typeSelected.toString() != "null" ? this.typeSelected : null;
    this.paginationInfo.actualPage = 0;
    this.getCatalog(this.paginationInfo.itemsPage, this.paginationInfo.actualPage, this.brandSelected, this.typeSelected);
    }

    onBrandFilterChanged(event: any, value: number) {
        event.preventDefault();
        this.brandSelected = value;
    }

    onTypeFilterChanged(event: any, value: number) {
        event.preventDefault();
        this.typeSelected = value;
    }

    onPageChanged(value: any) {
        console.log('catalog pager event fired' + value);
        event.preventDefault();
        this.paginationInfo.actualPage = value;
        this.getCatalog(this.paginationInfo.itemsPage, value);
    }

    addToCart(item: ICatalogItem) {
        this.basketService.addItemToBasket(item);
    }

    getCatalog(pageSize: number, pageIndex: number, brand?: number, type?: number) {
        this.errorReceived = false;
        this.service.getCatalog(pageIndex, pageSize, brand, type)
            .pipe(catchError((err) => this.handleError(err)))
            .subscribe(catalog => {
                this.catalog = catalog;
                this.paginationInfo = {
                    actualPage : catalog.pageIndex,
                    itemsPage : catalog.pageSize,
                    totalItems : catalog.count,
                    totalPages: Math.ceil(catalog.count / catalog.pageSize),
                    items: catalog.pageSize
                };
        });
    }

    getTypes() {
        this.service.getTypes().subscribe(types => {
            this.types = types;
            let alltypes = { id: null, type: 'All' };
            this.types.unshift(alltypes);
        });
    }

    getBrands() {
        this.service.getBrands().subscribe(brands => {
            this.brands = brands;
            let allBrands = { id: null, brand: 'All' };
            this.brands.unshift(allBrands);
        });
  }

  private handleError(error: any) {
        this.errorReceived = true;
        return Observable.throw(error);
    }
}

