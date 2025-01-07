export class CreateItemDto {
    Name!: string;
    Brand!: string;
    Description!: string;
    DateAcquired!: Date;
    Price!: number;
    SerialNumber!: string;
    ImageUrl!: string;
    ItemTypeId!: string; // Link to ItemType
    UserId!: string;

    // Watch-specific properties
    MovementType?: string;
    CaseMaterial?: string;
    CaseDiameter?: string;
    CaseThickness?: string;
    BandMaterial?: string;
    BandWidth?: string;

    // Cologne-specific properties
    FragranceNotes?: string;
    Concentration?: string;
    Type?: string;

    // Sneaker-specific properties
    Colorway?: string;
    Size?: string;

    constructor(init?: Partial<CreateItemDto>) {
        Object.assign(this, init);
    }
}

