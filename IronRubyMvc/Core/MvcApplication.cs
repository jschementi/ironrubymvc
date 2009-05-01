namespace System.Web.Mvc.IronRuby.Core
{
    public abstract class MvcApplication 
    {
        public virtual void Start(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void Error(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void AcquireRequestState(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void AuthenticateRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void AuthorizeRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void BeginRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void Disposed(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void EndRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void LogRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostAcquireRequestState(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void MapRequestHandler(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostAuthenticateRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostAuthorizeRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostLogRequest(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostMapRequestHandler(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostReleaseRequestState(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostRequestHandlerExecute(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostResolveRequestCache(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PostUpdateRequestCache(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PreSendRequestContent(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void PreSendRequestHeaders(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void ReleaseRequestState(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void ResolveRequestCache(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

        public virtual void UpdateRequestCache(object sender, EventArgs e)
        {
            //intentionally left blank for a better overriding experience
        }

    }
}