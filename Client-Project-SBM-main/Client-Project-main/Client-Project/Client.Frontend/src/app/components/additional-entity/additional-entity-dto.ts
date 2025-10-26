export interface AdditionalEntityGetDto {
    r_id: number;
    r_type: string;
    r_amount: number;
    r_quantity: number;
    r_date: number;
    r_companyId: number;
    r_subContractorName: string;
    r_subContractorId: number;
}

export interface AdditionalEntityCreateDto {
    type: string;
    amount: number;
    quantity: number;
    date: number;
    companyId: number;
    subContractorId: number;
    createdBy: number;
}

export interface AdditionalEntityUpdateDto {
    id: number;
    type: string;
    amount: number;
    quantity: number;
    date: number;
    companyId: number;
    subContractorId: number;
    updatedBy: number;
}
