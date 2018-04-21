/* Copyright (C) 2013 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skywolf.IBApi;

namespace Skywolf.TradingService.Messages
{
    public class ExecutionMessage
    {
        private int reqId;
        private Contract contract;
        private Execution execution;
        private CommissionReport commission;

        public ExecutionMessage(int reqId, Contract contract, Execution execution)
        {
            ReqId = reqId;
            Contract = contract;
            Execution = execution;
        }

        public Contract Contract
        {
            get { return contract; }
            set { contract = value; }
        }
        
        public Execution Execution
        {
            get { return execution; }
            set { execution = value; }
        }

        public int ReqId
        {
            get { return reqId; }
            set { reqId = value; }
        }

        public CommissionReport Commission
        {
            get { return commission; }
            set { commission = value; }
        }
    }
}
