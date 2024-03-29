﻿namespace WebApi.Entities {
    public class Transaction {
        public int Id { get; set; }
        public string Data { get; set; }

        public TransactionStatus Status { get; set; }
    }

    public enum TransactionStatus {
        Created,
        Processing,
        Processed,
        Aborted
    }
}
