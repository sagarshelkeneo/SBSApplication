export interface PaymentGetDto {
    r_id: number;
    r_invoiceNo: number;
    r_fromDate: string;
    r_toDate: string;
    r_paymentDate: string;
    r_amountPaid: number;
    r_paymentMode: string;
    r_bankName: string;
    r_bankId: number | null;
    r_paymentStatus: string;
}

export interface PaymentCreateDto {
    invoiceNo: number | null;
    companyId: number;
    fromDate: string | null;
    toDate: string | null;
    paymentDate: string;
    amountPaid: number;
    paymentMode: string
    bankId: number | null;
    paymentStatus: string;
    createdBy: number;
}

export interface PaymentUpdateDto {
    id: number;
    companyId: number;
    invoiceNo: number | null;
    paymentDate: string;
    amountPaid: number;
    paymentMode: string
    bankId: number | null;
    paymentStatus: string;
    updatedBy: number;
}