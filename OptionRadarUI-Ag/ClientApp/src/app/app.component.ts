import { AfterViewInit, Component, Renderer2 } from '@angular/core';
import { LoaderService } from './services/loader.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements AfterViewInit {

  constructor(private loaderService: LoaderService, private renderer: Renderer2) { }

  ngAfterViewInit() {
    this.loaderService.httpProgress().subscribe((status: boolean) => {
      if (status) {
        this.renderer.addClass(document.body, 'cursor-loader');
      } else {
        this.renderer.removeClass(document.body, 'cursor-loader');
      }
    });
  }

  title = 'app';

  columnDefs = [
    { headerName: 'USymbol', field: 'USymbol', sortable: true, filter: 'agTextColumnFilter' },
    { headerName: 'OSymbol', field: 'OSymbol', sortable: true, filter: 'agTextColumnFilter' },
    { headerName: 'Strike', field: 'Strike', sortable: true, filter: 'agNumberColumnFilter' },
    { headerName: 'ContractType', field: 'ContractType', sortable: true, filter: 'agTextColumnFilter' },
    {
      headerName: 'TotalVolume', field: 'TotalVolume', sortable: true, filter: 'agNumberColumnFilter'
    },
    {
      headerName: 'OpenInterest', field: 'OpenInterest', sortable: true, filter: 'agNumberColumnFilter'
    },
    { headerName: 'VolumeDelta', field: 'VolumeDelta', sortable: true },
    { headerName: 'OIDelta', field: 'OIDelta', sortable: true },
    { headerName: 'ExpirationDate', field: 'Expiry', sortable: true },
    { headerName: 'Delta', field: 'Delta', sortable: true }
  ];

  rowData = [];

  ngOnInit() {
    fetch('/api/OptionsActivity/GetUnusualActivity')
      .then(result => result.json())
      .then(rowData => this.rowData = rowData);
  }
}
