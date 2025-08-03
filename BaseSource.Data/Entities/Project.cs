using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string WebhookUrl { get; set; }
        public bool IsActive { get; set; }

        public string UserId { get; set; }
        public UserProfile User { get; set; }

        public ICollection<WebhookHeader> Headers { get; set; }
        public ICollection<CryptoWallet> Wallets { get; set; }
    }

    public class WebhookHeader
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }


    public class CryptoWallet
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public EChain Chain { get; set; }
        public bool IsActive { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public ICollection<BlockchainTransaction> Transactions { get; set; }
    }

    public class BlockchainTransaction
    {
        public int Id { get; set; }
        public int CryptoWalletId { get; set; }
        public CryptoWallet CryptoWallet { get; set; }

        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
        public string TxHash { get; set; }
        public string TokenSymbol { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public ICollection<WebhookHistory> WebhookHistories { get; set; }
    }

    public class WebhookHistory
    {
        public int Id { get; set; }

        public int BlockchainTransactionId { get; set; }
        public BlockchainTransaction BlockchainTransaction { get; set; }

        public int RetryCount { get; set; }
        public bool IsSuccess { get; set; }
        public int ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public DateTime SentAt { get; set; }
    }
}
