﻿using System;
using System.Collections.ObjectModel;

namespace BlockChain.WPF.Services {
    public class ReadTransaction{

        public ReadTransaction(BlockContainer blocks, ObservableCollection<string> messages){
            _blocks = blocks;
            _messages = messages;
        }

        readonly ObservableCollection<string> _messages;
        readonly BlockContainer _blocks;

        public void Execute(string txId){

            var transaction = _blocks[txId];

            if (transaction == null) {
                _messages.Add("Transaction not found");
                return;
            }

            _messages.Add($"{Environment.NewLine}");
            _messages.Add($"Transaction: {transaction}");
            _messages.Add($"Fee Amount: {transaction.Fees}");
            _messages.Add($"Fee Per Out: {transaction.FeePerOut}");

            _messages.Add($"{Environment.NewLine}");
            _messages.Add("In Transactions");
            _messages.Add($"In Amount: {transaction.Ins.Amount}");

            foreach (var txIn in transaction.Ins) {
                _messages.Add($"\t#{txIn.PreviousOutput.N}-{txIn.PreviousOutput.TransactionHash}");
            }

            _messages.Add($"{Environment.NewLine}");
            _messages.Add("Out Transactions");
            _messages.Add($"Out Amount: {transaction.Outs.Amount}");

            foreach (var txOut in transaction.Outs) {
                _messages.Add($"\t#{txOut.N}-{txOut.Value}-{txOut.Script.OpString}");
            }
        }
    }
}