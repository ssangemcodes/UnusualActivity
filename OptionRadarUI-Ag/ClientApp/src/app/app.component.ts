import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
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
    { headerName: 'ExpirationDate', field: 'Expiry', sortable: true },
    { headerName: 'Delta', field: 'Delta', sortable: true }
  ];

  rowData = [];

  ngOnInit() {
    fetch('/OptionsActivity/GetUnusualActivity')
      .then(result => result.json())
      .then(rowData => this.rowData = rowData);
  }
}
