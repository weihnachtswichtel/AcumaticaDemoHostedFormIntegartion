﻿using Acumatica.ADPCGateway;
using Acumatica.ADPCGateway.Model;
using PX.CCProcessingBase.Interfaces.V2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcumaticaDummyCreditCardPlugin
{
    public class ADCPTransactionProcessor : ICCTransactionProcessor
    {
        private IEnumerable<SettingsValue> settingValues;

        public ADCPTransactionProcessor(IEnumerable<SettingsValue> settingValues)
        {
            this.settingValues = settingValues;
        }

        public ProcessingResult DoTransaction(ProcessingInput inputData)
        {
            //Here can be implemented the API call to the processing center and passing the all data including
            //inputData.TranUID that should be stored on Processing Center side same way as in scenario with HostedPaymentForm
            //(Implementation of GetDataForPaymentForm method of ICCHostedPaymentFormProcessor interface)

            Transaction tranToCreate = new Transaction();

            // tran.CustomerProfileID = inputData.CustomerData.CustomerProfileID;
            tranToCreate.PaymentProfileID = Guid.Parse(inputData.CardData.PaymentProfileID);
            tranToCreate.TransactionDocument = inputData.DocumentData.DocType + inputData.DocumentData.DocRefNbr;
            tranToCreate.TransactionType = ADCPHelper.MapTranType.FirstOrDefault(x => x.Value == inputData.TranType).Key;
            tranToCreate.TransactionAmount = inputData.Amount;
            tranToCreate.Tranuid = inputData.TranUID;
            tranToCreate.TransactionStatus = "Approved";
            tranToCreate.TransactionCurrency = inputData.CuryID;

            Transaction tran = Requests.CreateTransaction(ADCPHelper.GetPCGredentials(settingValues), tranToCreate);

            ProcessingResult processingResult = new ProcessingResult {
                TransactionNumber = tran.TransactionID,
                ResponseCode = "200",
                ResponseText = "Success",
                ResponseReasonCode = "200",
                ResponseReasonText = "Success",
                AuthorizationNbr = tran.AuthorizationNbr,
                ExpireAfterDays = 30, //tran.TransactionExpirationDate == null || tran.TransactionDate == null? 30 : (tran.TransactionExpirationDate.Value-tran.TransactionDate).Value.Days,
                CcvVerificatonStatus = CcvVerificationStatus.Match,
            };
            return processingResult;
        }
    }
}