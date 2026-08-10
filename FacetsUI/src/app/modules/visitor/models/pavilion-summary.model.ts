import { PavilionSessionSummaryModel } from "./pavilion-session-summary.model";

export class PavilionSummaryModel {
    id: string;
    name: string;
    numberOfSession: number;
    status: string;
    pavilionSessionSummaries = new Array<PavilionSessionSummaryModel>();
}