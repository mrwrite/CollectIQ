import { Item } from "./item.model";

export class ItemStats {
    type!: string;
    count!: number;
    items: Item[] = [];
    itemTypeId!: string;
    showItems: boolean = false;
  }