import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.html',
  styleUrls: ['./categories.scss'],
  standalone: false
})
export class CategoriesComponent implements OnInit {
  categories: any[] = [];
  showModal = false;
  isEditing = false;
  currentCategory: any = { id: 0, name: '', description: '' };

  constructor(private categoryService: CategoryService) {}

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe((res: any) => {
      this.categories = res;
    });
  }

  addCategory() {
    this.isEditing = false;
    this.currentCategory = { id: 0, name: '', description: '' };
    this.showModal = true;
  }

  editCategory(category: any) {
    this.isEditing = true;
    this.currentCategory = { ...category };
    this.showModal = true;
  }

  deleteCategory(category: any) {
    this.categoryService.deleteCategory(category.id).subscribe(() => {
      this.loadCategories();
    });
  }

  saveCategory() {
    if (this.isEditing) {
      this.categoryService.updateCategory(this.currentCategory.id, this.currentCategory)
        .subscribe(() => this.loadCategories());
    } else {
      this.categoryService.addCategory(this.currentCategory)
        .subscribe(() => this.loadCategories());
    }
    this.showModal = false;
  }

  closeModal() {
    this.showModal = false;
  }
}
