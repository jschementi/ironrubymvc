/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * ironruby@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

[assembly: IronRuby.Runtime.RubyLibraryAttribute(typeof(IronRubyMvcLibrary.IronRubyMvcLibraryLibraryInitializer))]

namespace IronRubyMvcLibrary {
    public sealed class IronRubyMvcLibraryLibraryInitializer : IronRuby.Builtins.LibraryInitializer {
        protected override void LoadModules() {
            IronRuby.Builtins.RubyClass classRef0 = GetClass(typeof(System.Web.Mvc.ControllerBase));
            
            
            IronRuby.Builtins.RubyModule def1 = DefineGlobalModule("IronRubyMvc", typeof(IronRubyMvcLibrary.Controllers.IronRubyMvcModule), true, null, null, null, IronRuby.Builtins.RubyModule.EmptyArray);
            IronRuby.Builtins.RubyClass def3 = DefineGlobalClass("RubyController", typeof(System.Web.Mvc.Controller), false, classRef0, null, null, null, IronRuby.Builtins.RubyModule.EmptyArray, 
                new System.Func<IronRuby.Builtins.RubyClass, IronRubyMvcLibrary.Controllers.RubyController>(IronRubyMvcLibrary.Controllers.RubyController.Create), 
                new System.Func<IronRuby.Builtins.RubyClass, System.String, IronRubyMvcLibrary.Controllers.RubyController>(IronRubyMvcLibrary.Controllers.RubyController.Create)
            );
            IronRuby.Builtins.RubyClass def2 = DefineClass("IronRubyMvc::Controller", typeof(IronRubyMvcLibrary.Controllers.RubyController), false, def3, LoadIronRubyMvc__Controller_Instance, null, null, IronRuby.Builtins.RubyModule.EmptyArray);
            def1.SetConstant("Controller", def2);
        }
        
        private static void LoadIronRubyMvc__Controller_Instance(IronRuby.Builtins.RubyModule/*!*/ module) {
            module.DefineLibraryMethod("info", 0x11, 
                new System.Action<IronRubyMvcLibrary.Controllers.RubyController>(IronRubyMvcLibrary.Controllers.IronRubyMvcModule.RubyControllerOps.Info)
            );
            
        }
        
    }
}

