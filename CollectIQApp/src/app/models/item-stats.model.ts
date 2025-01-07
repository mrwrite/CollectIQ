import { Item } from "./item.model";

export class ItemStats {
    type!: string;
    count!: number;
    items: Item[] = [];
    showItems: boolean = false;
  }