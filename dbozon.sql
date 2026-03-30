CREATE TYPE event_type AS ENUM (
    'view',
    'purchase'
);

CREATE TYPE report_request_status AS ENUM (
    'pending',
    'processing',
    'done',
    'failed'
);

CREATE TABLE Events (
    Id uuid PRIMARY KEY,
    ProductId uuid NOT NULL,
    EventTime timestamp with time zone NOT NULL,
    EventType event_type NOT NULL,
	CheckoutId uuid NOT NULL
);

CREATE INDEX idx_events_product_time_type
ON Events(ProductId, EventTime, EventType, CheckoutId);

CREATE TABLE Reports (
    Id uuid PRIMARY KEY,
    ProductId uuid NOT NULL,
    DateFrom timestamp with time zone NOT NULL,
    DateTo timestamp with time zone NOT NULL,
    Ratio decimal(10, 4) NOT NULL,
	CheckoutId uuid NOT NULL,
    PurchasesCount int NOT NULL,
	ViewsCount int NOT NULL,
    CreatedAt timestamp with time zone NOT NULL
);

CREATE UNIQUE INDEX ux_reports_unique
ON Reports(ProductId, DateFrom, DateTo, CheckoutId);

CREATE INDEX idx_reports_lookup
ON Reports(ProductId, DateFrom, DateTo, CheckoutId);

CREATE TABLE ReportRequests (
    Id uuid PRIMARY KEY,
    ProductId uuid NOT NULL,
    DateFrom timestamp with time zone NOT NULL,
    DateTo timestamp with time zone NOT NULL,
    CheckoutId uuid NOT NULL,
    Status report_request_status NOT NULL,
    ReportId uuid NULL,
    CreatedAt timestamp with time zone NOT NULL,

    CONSTRAINT fk_report_requests_report
        FOREIGN KEY (ReportId) REFERENCES Reports(Id)
);

CREATE UNIQUE INDEX ux_reportrequests_unique
ON ReportRequests(ProductId, DateFrom, DateTo, CheckoutId);

CREATE INDEX idx_reportrequests_status
ON ReportRequests(Status);

CREATE INDEX idx_reportrequests_pending
ON ReportRequests(Status)
WHERE Status = 'pending';

CREATE UNIQUE INDEX ux_reportrequests_reportid
ON ReportRequests(ReportId)
WHERE ReportId IS NOT NULL;