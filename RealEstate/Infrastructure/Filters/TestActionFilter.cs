﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RealEstate.Messaging;

namespace RealEstate.Infrastructure.Filters
{
    public class TestActionFilter : ActionFilterAttribute
    {
        public MessageBus MessageBus { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MessageBus.Publish(new TestMessage(this, MessageType.A, "hello from action filter"));
            
            base.OnActionExecuting(filterContext);
        }
    }
}