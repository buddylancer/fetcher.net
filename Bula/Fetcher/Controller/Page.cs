// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using System.Collections;
    using Bula.Fetcher;
    using Bula.Objects;

    /// <summary>
    /// Basic logic for generating Page block.
    /// </summary>
    public abstract class Page : Bula.Meta {
        /// Current context 
        protected Context context = null;

        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Page (Context context) {
            this.context = context;
            //echo "In Page constructor -- " . Print_r(context, true);
        }

        /// Execute main logic for page block 
        abstract public void Execute();

        /// <summary>
        /// Merge template with variables and write to engine.
        /// </summary>
        /// <param name="template">Template name.</param>
        /// <param name="prepare">Prepared variables.</param>
        public void Write(String template, Hashtable prepare) {
            var engine = this.context.GetEngine();
            engine.Write(engine.ShowTemplate(template, prepare));
        }
    }
}