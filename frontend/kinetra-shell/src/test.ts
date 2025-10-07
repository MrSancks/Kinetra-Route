import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import {
  BrowserDynamicTestingModule,
  platformBrowserDynamicTesting
} from '@angular/platform-browser-dynamic/testing';

beforeAll(() => {
  platformBrowserDynamicTesting().bootstrapModule(BrowserDynamicTestingModule);
});

afterAll(() => {
  getTestBed().resetTestingModule();
});
