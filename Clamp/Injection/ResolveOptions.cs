﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Injection
{
    public sealed class ResolveOptions
    {
        private static readonly ResolveOptions _Default = new ResolveOptions();
        private static readonly ResolveOptions _FailUnregisteredAndNameNotFound = new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail };
        private static readonly ResolveOptions _FailUnregisteredOnly = new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail };
        private static readonly ResolveOptions _FailNameNotFoundOnly = new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve };

        private UnregisteredResolutionActions _UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve;
        public UnregisteredResolutionActions UnregisteredResolutionAction
        {
            get { return _UnregisteredResolutionAction; }
            set { _UnregisteredResolutionAction = value; }
        }

        private NamedResolutionFailureActions _NamedResolutionFailureAction = NamedResolutionFailureActions.Fail;
        public NamedResolutionFailureActions NamedResolutionFailureAction
        {
            get { return _NamedResolutionFailureAction; }
            set { _NamedResolutionFailureAction = value; }
        }

        /// <summary>
        /// Gets the default options (attempt resolution of unregistered types, fail on named resolution if name not found)
        /// </summary>
        public static ResolveOptions Default
        {
            get
            {
                return _Default;
            }
        }

        /// <summary>
        /// Preconfigured option for attempting resolution of unregistered types and failing on named resolution if name not found
        /// </summary>
        public static ResolveOptions FailNameNotFoundOnly
        {
            get
            {
                return _FailNameNotFoundOnly;
            }
        }

        /// <summary>
        /// Preconfigured option for failing on resolving unregistered types and on named resolution if name not found
        /// </summary>
        public static ResolveOptions FailUnregisteredAndNameNotFound
        {
            get
            {
                return _FailUnregisteredAndNameNotFound;
            }
        }

        /// <summary>
        /// Preconfigured option for failing on resolving unregistered types, but attempting unnamed resolution if name not found
        /// </summary>
        public static ResolveOptions FailUnregisteredOnly
        {
            get
            {
                return _FailUnregisteredOnly;
            }
        }
    }
}
