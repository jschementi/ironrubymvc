#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class RubyControllerActionInvoker : ControllerActionInvoker
    {
        public RubyControllerActionInvoker(string controllerName, IRubyEngine engine)
        {
            ControllerName = controllerName;
            RubyEngine = engine;
        }

        public string ControllerName { get; private set; }

        public IRubyEngine RubyEngine { get; private set; }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            var rubyController = (RubyController) controllerContext.Controller;
            return new RubyControllerDescriptor(rubyController.RubyType) {RubyEngine = RubyEngine};
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext,
                                                       ControllerDescriptor controllerDescriptor, string actionName)
        {
            return controllerDescriptor.FindAction(controllerContext, actionName);
        }

        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var rubyType = ((RubyController) controllerContext.Controller).RubyType;
            var actionFilters = (Hash) RubyEngine.CallMethod(rubyType, "action_filters");
            var filters = new RubyFilterInfo(actionFilters);
            
            return filters;
        }

        private static void AddControllerToFilterList<TFilter>(ControllerBase controller, IList<TFilter> filterList)
            where TFilter : class
        {
            var controllerAsFilter = controller as TFilter;
            if (controllerAsFilter != null)
            {
                filterList.Insert(0, controllerAsFilter);
            }
        }

        public override bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (String.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("actionName");
            }

            var controllerDescriptor = GetControllerDescriptor(controllerContext);
            var actionDescriptor = FindAction(controllerContext, controllerDescriptor, actionName);
            if (actionDescriptor != null)
            {
                var filterInfo = GetFilters(controllerContext, actionDescriptor);

                try
                {
                    var authContext = InvokeAuthorizationFilters(controllerContext, filterInfo.AuthorizationFilters,
                                                                 actionDescriptor);
                    if (authContext.Result != null)
                    {
                        // the auth filter signaled that we should let it short-circuit the request
                        InvokeActionResult(controllerContext, authContext.Result);
                    }
                    else
                    {
                        if (controllerContext.Controller.ValidateRequest)
                        {
                            ValidateRequest(controllerContext.HttpContext.Request);
                        }

                        var parameters = GetParameterValues(controllerContext, actionDescriptor);
                        var postActionContext = InvokeActionMethodWithFilters(controllerContext,
                                                                              filterInfo.ActionFilters, actionDescriptor,
                                                                              parameters);
                        InvokeActionResultWithFilters(controllerContext, filterInfo.ResultFilters,
                                                      postActionContext.Result);
                    }
                }
                catch (ThreadAbortException)
                {
                    // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                    // the filters don't see this as an error.
                    throw;
                }
                catch (Exception ex)
                {
                    // something blew up, so execute the exception filters
                    var exceptionContext = InvokeExceptionFilters(controllerContext, filterInfo.ExceptionFilters, ex);
                    if (!exceptionContext.ExceptionHandled)
                    {
                        throw;
                    }
                    InvokeActionResult(controllerContext, exceptionContext.Result);
                }

                return true;
            }

            // notify controller that no method matched
            return false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "rawUrl",
            Justification = "We only care about the property getter's side effects, not the returned value.")]
        private static void ValidateRequest(HttpRequestBase request)
        {
            // DevDiv 214040: Enable Request Validation by default for all controller requests
            // 
            // Note that we grab the Request's RawUrl to force it to be validated. Calling ValidateInput()
            // doesn't actually validate anything. It just sets flags indicating that on the next usage of
            // certain inputs that they should be validated. We special case RawUrl because the URL has already
            // been consumed by routing and thus might contain dangerous data. By forcing the RawUrl to be
            // re-read we're making sure that it gets validated by ASP.NET.

            request.ValidateInput();
            var rawUrl = request.RawUrl;
        }
    }
}