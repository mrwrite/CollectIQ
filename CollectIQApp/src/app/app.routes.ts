import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuard } from './services/auth.guard';
import { ItemListComponent } from './item-list/item-list.component';
import { DashboardHomeComponent } from './dashboard/dashboard-home/dashboard-home.component';
import { UserItemsListComponent } from './dashboard/user-items-list/user-items-list.component';
import { SettingsComponent } from './dashboard/settings/settings.component';
import { UsersComponent } from './dashboard/users/users.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent},
    { 
        path: 'dashboard', 
        component: DashboardComponent, 
        canActivate: [AuthGuard],
        children: [
            {path: '', redirectTo: 'dashboard-home', pathMatch: 'full'},
            {path: 'dashboard-home', component: DashboardHomeComponent},
            {path: 'user-items-list', component: ItemListComponent},
            {path: 'users', component: UsersComponent},
            {path: 'settings', component: SettingsComponent}
        ],
    },
    { path: 'items/:type', component: ItemListComponent, canActivate: [AuthGuard] },
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: '**', redirectTo: '/login'}
];
