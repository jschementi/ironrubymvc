#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Web.Mvc.IronRuby.Helpers")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Web.Mvc.IronRuby.ViewEngine")]
[assembly: SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
[assembly: SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
[assembly: SuppressMessage("Microsoft.Design",
    "CA1061:DoNotHideBaseClassMethods",
    Scope = "member",
    Target = "System.Web.Mvc.IronRuby.Controllers.RubyController.#View(System.Object)",
    Justification = "Hiding is intended")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Scope = "member", Target = "System.Web.Mvc.IronRuby.Controllers.RubyController.#View(System.String,System.Object)",
        Justification = "Hiding is intended")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "E", Scope = "member", Target = "System.Web.Mvc.IronRuby.Helpers.RubyUrlHelper.#E(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "E", Scope = "member", Target = "System.Web.Mvc.IronRuby.Helpers.RubyUrlHelper.#E(System.String)")]