export class DecodedToken{
    nameidentifier: string;
    UserId: string
    unique_name: string;
    role: string;
    nbf: number;
    exp: number;
    iat: number;
    sub: string;


    constructor(nameidentifier: string, UserId: string, unique_name: string, role: string, nbf: number, exp: number, iat: number, sub: string){
        this.nameidentifier = nameidentifier;
        this.unique_name = unique_name;
        this.role = role;
        this.nbf = nbf;
        this.exp = exp;
        this.iat = iat;
        this.sub = sub;
        this.UserId = UserId;
    }
}