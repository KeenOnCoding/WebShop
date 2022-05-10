import { NgModule }             from '@angular/core';
import { BrowserModule  }       from '@angular/platform-browser';
import { CommonModule }         from '@angular/common'
import { SharedModule }         from '../shared/shared.module';
import { CatalogComponent }     from './catalog.component';
import { CatalogService }       from './catalog.service';
import { Pager }                from '../shared/components/pager/pager';
import { BasketModule } from '../basket/basket.module';
import { AutocompleteLibModule } from '../autocomplete/autocomplete-lib-module.module';


@NgModule({
  imports: [BrowserModule, SharedModule, CommonModule, BasketModule,  AutocompleteLibModule],
    declarations: [CatalogComponent],
    providers: [CatalogService]
})
export class CatalogModule { }
