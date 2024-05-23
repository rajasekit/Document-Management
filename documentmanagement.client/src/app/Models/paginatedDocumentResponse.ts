import { document } from "./document";

export interface paginatedDocumentResponse {
    documents: document[];
    totalCount: number;
    }