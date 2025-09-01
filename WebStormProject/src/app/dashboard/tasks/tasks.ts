import { Component, OnInit } from '@angular/core';
import { TaskService } from '../../services/task';

interface Category {
  id: number;
  name: string;
}

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.html',
  styleUrls: ['./tasks.scss'],
  standalone: false
})
export class TasksComponent implements OnInit {
  tasks: any[] = [];
  statuses = ['InProgress', 'Completed', 'Canceled'];
  priorities = ['High', 'Low'];
  userCategories: Category[] = [];
  filters = { status: '', category: '', priority: '' };

  showModal = false;
  isEditing = false;
  currentTask: any = {
    id: 0,
    title: '',
    description: '',
    dueDate: '',
    priority: 'High',
    status: 'InProgress',
    categoryId: 0  // هنا تخزن الـ ID للكاتيجوري بدل الاسم
  };



  constructor(private taskService: TaskService) {}

  ngOnInit() {
    this.loadCategories();
    this.loadTasks();
  }

  loadCategories() {
    this.taskService.getUserCategories().subscribe(categories => {
      this.userCategories = categories;
      if (categories.length > 0 && !this.currentTask.categoryId) {
        this.currentTask.categoryId = categories[0].id;
      }
    });
  }

  loadTasks() {
    this.taskService.getTasks().subscribe(tasks => {
      this.tasks = tasks.map(t => ({
        ...t,
        categoryName: t.category?.name || ''
      }));
    });
  }

  addTask() {
    this.isEditing = false;
    this.showModal = true;
    this.currentTask = {
      id: 0,
      title: '',
      description: '',
      dueDate: '',
      priority: 'High',
      categoryId: this.userCategories.length > 0 ? this.userCategories[0].id : 0,
      status: 'InProgress'
    };
  }

  editTask(task: any) {
    this.isEditing = true;
    this.currentTask = { ...task };
    this.showModal = true;
  }

  deleteTask(task: any) {
    this.taskService.deleteTask(task.id).subscribe(() => {
      this.tasks = this.tasks.filter(t => t.id !== task.id);
    });
  }

  saveTask() {
    if (!this.currentTask.categoryId) {
      alert('Please select a valid category!');
      return;
    }

    if (this.isEditing) {
      this.taskService.updateTask(this.currentTask.id, this.currentTask)
        .subscribe(updated => {
          const index = this.tasks.findIndex(t => t.id === updated.id);
          if (index > -1) this.tasks[index] = { ...updated, categoryName: updated.category?.name || '' };
          this.showModal = false;
        });
    } else {
      this.taskService.addTask(this.currentTask)
        .subscribe(created => {
          this.tasks.push({ ...created, categoryName: created.category?.name || '' });
          this.showModal = false;
        });
    }
  }

  closeModal() {
    this.showModal = false;
  }

  markNextStatus(task: any) {
    const nextStatus = task.status === 'InProgress' ? 'Completed' : task.status === 'Completed' ? 'Canceled' : null;
    if (nextStatus) {
      task.status = nextStatus;
      this.taskService.updateTask(task.id, task).subscribe(updated => {
        const index = this.tasks.findIndex(t => t.id === updated.id);
        if (index > -1) this.tasks[index].status = updated.status;
      });
    }
  }

  getStatusColor(status: string) {
    switch (status) {
      case 'InProgress': return '#fbb8d1';
      case 'Completed': return '#7bbef5';
      case 'Canceled': return '#ccc';
      default: return '#ccc';
    }
  }
// داخل TasksComponent
  public get filteredTasks() {
    return this.tasks.filter(task =>
      (!this.filters.status || task.status === this.filters.status) &&
      (!this.filters.category || task.categoryName === this.filters.category) &&
      (!this.filters.priority || task.priority === this.filters.priority)
    );
  }

  getPriorityColor(priority: string) {
    switch (priority) {
      case 'High': return '#7bbef5';
      case 'Low': return '#fbb8d1';
      default: return '#ccc';
    }
  }
}
