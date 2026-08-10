export class ErrorResponse {
    traceId: string | undefined | null;
    success: boolean;
    errors: [{ key: string; value: [string] }];
  }