import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserItemsListComponent } from './user-items-list.component';

describe('UserItemsListComponent', () => {
  let component: UserItemsListComponent;
  let fixture: ComponentFixture<UserItemsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserItemsListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserItemsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
