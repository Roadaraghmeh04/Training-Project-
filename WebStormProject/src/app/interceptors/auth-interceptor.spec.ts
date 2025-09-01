import { TestBed } from '@angular/core/testing';
import { AuthInterceptor } from './auth-interceptor';
import { AuthService } from '../services/auth';
import { HttpRequest, HttpHandler } from '@angular/common/http';

describe('AuthInterceptor', () => {
  let interceptor: AuthInterceptor;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('AuthService', ['getToken']);

    TestBed.configureTestingModule({
      providers: [
        AuthInterceptor,
        { provide: AuthService, useValue: spy }
      ]
    });

    interceptor = TestBed.inject(AuthInterceptor);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  it('should be created', () => {
    expect(interceptor).toBeTruthy();
  });

  it('should add Authorization header if token exists', () => {
    const token = 'my-token';
    authServiceSpy.getToken.and.returnValue(token);

    const req = new HttpRequest('GET', '/test');
    const next: HttpHandler = {
      handle: (request: HttpRequest<any>) => {
        expect(request.headers.get('Authorization')).toBe(`Bearer ${token}`); // ✅ backticks
        return { subscribe: () => {} } as any; // dummy observable
      }
    };

    interceptor.intercept(req, next).subscribe();
  });

  it('should not add Authorization header if no token', () => {
    authServiceSpy.getToken.and.returnValue(null);

    const req = new HttpRequest('GET', '/test');
    const next: HttpHandler = {
      handle: (request: HttpRequest<any>) => {
        expect(request.headers.has('Authorization')).toBeFalse();
        return { subscribe: () => {} } as any; // dummy observable
      }
    };

    interceptor.intercept(req, next).subscribe();
  });
});
