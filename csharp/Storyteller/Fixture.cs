﻿
using Baseline;
using Storyteller.Model.Lists;

namespace Storyteller
{

    public class Fixture
    {
        protected internal readonly LightweightCache<string, OptionList> Lists =
            new LightweightCache<string, OptionList>(key => new OptionList(key));

        
        public string Key { get; protected set; }
        
        
        /*
        
        
        private static readonly List<Type> _ignoredTypes = new List<Type>
        {
            typeof(object),
            typeof(Fixture)
        };

        private readonly LightweightCache<string, IGrammar> _grammars;


        public string Title;

        public Fixture()
        {
            _grammars = new LightweightCache<string, IGrammar>(findGrammar);

            Key = GetType().Name.Replace("Fixture", "");

            this["TODO"] = Do<string>("TODO: {message}", StoryTellerAssert.Fail);
        }

        // SAMPLE: fixture-grammars
        [IndexerName("Grammars")]
        public IGrammar this[string key]
        {
            get { return _grammars[key]; }
            set
            {
                _grammars[key] = value;
                value.Key = key;
            }
        }

        /// <summary>
        ///     The currently executing specification context. This property will only
        ///     be set at runtime during specification runs
        /// </summary>
        public IExecutionContext Context { get; set; }

        

        /// <summary>
        ///     Shortcut to get or set the current object on the context state
        /// </summary>
        public object CurrentObject
        {
            get { return Context?.State.CurrentObject; }
            set
            {
                if (Context != null) Context.State.CurrentObject = value;
            }
        }

        // ENDSAMPLE

        /// <summary>
        ///     Shorthand for Context.Service&lt;T&gt;()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Retrieve<T>()
        {
            return Context.Services.GetService<T>();
        }
        */

        /// <summary>
        ///     Executes before any steps within a section using this Fixture object
        ///     Context will be available in this method
        /// </summary>
        public virtual void SetUp()
        {
        }

        /// <summary>
        ///     Executes after any steps within a section using this Fixture object
        ///     Context will be available in this method
        /// </summary>
        public virtual void TearDown()
        {
        }

        /*
        /// <summary>
        ///     All the grammars, including "hidden" grammars, in this Fixture
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGrammar> AllGrammars()
        {
            return _grammars;
        }

        public bool IsHidden()
        {
            return GetType().HasAttribute<HiddenAttribute>();
        }

        /// <summary>
        ///     Used internally to build the fixture model that the Storyteller client
        ///     uses to render the screens
        /// </summary>
        /// <param name="cellHandling"></param>
        /// <returns></returns>
        public virtual FixtureModel Compile(CellHandling cellHandling)
        {
            GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(methodFromThis)
                .Where(x => !x.HasAttribute<HiddenAttribute>())
                .Where(x => !x.IsSpecialName)
                .Each(method =>
                {
                    var grammarKey = method.GetKey();
                    if (_grammars.Has(grammarKey)) return;

                    var grammar = GrammarBuilder.BuildGrammar(method, this);
                    grammar.Key = grammarKey;
                    this[grammarKey] = grammar;
                });

            var grammars = new List<GrammarModel>();
            _grammars.Each((key, grammar) =>
            {
                var model = grammar.Compile(this, cellHandling);
                model.key = key;
                model.IsHidden = grammar.IsHidden;

                grammars.Add(model);
            });

            return new FixtureModel(Key)
            {
                title = Title ?? Key.SplitCamelCase(),
                grammars = grammars.Where(x => !x.IsHidden).ToArray(),
                implementation = GetType().FullName
            };
        }

        public IGrammar GrammarFor(string key)
        {
            return _grammars[key];
        }

        private IGrammar findGrammar(string key)
        {
            var method = GetType().GetMethod(key);


            if (method == null)
                return new MissingGrammar(key);

            var grammar = GrammarBuilder.BuildGrammar(method, this);
            grammar.Key = key;

            return grammar;
        }

        private static bool methodFromThis(MethodInfo method)
        {
            if (method.Name == "TODO") return true;

            if (_ignoredTypes.Contains(method.DeclaringType))
                return false;

            if (method.GetBaseDefinition() != null)
            {
                var declaringType = method.GetBaseDefinition().DeclaringType;
                if (_ignoredTypes.Contains(declaringType))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates a simple Sentence grammar with no inputs that executes an Action lambda
        /// </summary>
        /// <param name="text"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ActionGrammar Do(string text, Action<IExecutionContext> action)
        {
            return new ActionGrammar(text, action);
        }

        /// <summary>
        ///     Creates a simple Sentence grammar with one input that executes an Action<T> lambda"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="action"></param>
        /// <example>
        ///     this["simple"] = Do<int>("Add {x} to our number", x => count += x);
        /// </example>
        /// <returns></returns>
        public static ActionGrammar<T> Do<T>(string template, Action<T> action)
        {
            return new ActionGrammar<T>(template, (x, context) => action(x));
        }

        /// <summary>
        ///     Creates a simple Sentence grammar with one input that executes an Action<T, ITestContext> lambda
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ActionGrammar<T> Do<T>(string template, Action<T, IExecutionContext> action)
        {
            return new ActionGrammar<T>(template, action);
        }

        /// <summary>
        ///     Creates a simple Sentence grammar with one input that invokes a Lambda against a service object registered
        ///     in the current ITestContext.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="template"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ActionGrammar<TInput> Do<TInput, TService>(string template, Action<TInput, TService> action)
        {
            return new ActionGrammar<TInput>(template, (input, c) => action(input, c.Service<TService>()));
        }

        /// <summary>
        ///     Creates a grammar that checks the single value returned by
        ///     the Func[T].  Mostly useful for building up scripted
        ///     grammars
        /// </summary>
        /// <example>
        ///     return Script("Divide numbers", x =>
        ///     {
        ///     x += Do(() => _first = _second = 0);
        ///     x += Read
        ///     <double>
        ///         ("x", o => _first = o);
        ///         x += Read
        ///         <double>
        ///             ("y", o => _second = o);
        ///             x += Check("product", () => _first/_second);
        ///             }).AsTable("Subtract numbers");
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static CheckGrammar<T> Check<T>(string key, Func<T> result)
        {
            return Check(key, c => result());
        }

        /// <summary>
        ///     Creates a grammar that checks the single value returned by
        ///     the Func[T].  Mostly useful for building up scripted
        ///     grammars
        /// </summary>
        /// <example>
        ///     return Script("Divide numbers", x =>
        ///     {
        ///     x += Do(() => _first = _second = 0);
        ///     x += Read
        ///     <double>
        ///         ("x", o => _first = o);
        ///         x += Read
        ///         <double>
        ///             ("y", o => _second = o);
        ///             x += Check("product", () => _first/_second);
        ///             }).AsTable("Subtract numbers");
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static CheckGrammar<T> Check<T>(string key, Func<IExecutionContext, T> result)
        {
            return new CheckGrammar<T>(key, result);
        }

        /// <summary>
        ///     Use to create a simple "Fact" grammar that asserts
        ///     that a condition is true
        /// </summary>
        /// <example>
        ///     this["ThisFactIsTrue"] = Fact("This fact is always true").VerifiedBy(() => true);
        /// </example>
        /// <param name="title"></param>
        /// <returns></returns>
        protected FactExpression Fact(string title)
        {
            return new FactExpression(title);
        }

        /// <summary>
        ///     Creates a new embedded section grammar for the given title and fixture
        ///     type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <returns></returns>
        public static EmbeddedSectionGrammar<T> Embed<T>(string title) where T : Fixture, new()
        {
            return new EmbeddedSectionGrammar<T>
            {
                Title = title
            };
        }

        /// <summary>
        ///     Verify a list of string values
        /// </summary>
        /// <typeparam name="T">An application service that will be invoked to get the data</typeparam>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static VerifyStringListExpression VerifyStringList<T>(Func<T, IEnumerable<string>> dataSource)
        {
            return VerifyStringList(c => dataSource(c.Service<T>()));
        }

        /// <summary>
        ///     Verify a list of string values
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static VerifyStringListExpression VerifyStringList(Func<IExecutionContext, IEnumerable<string>> dataSource)
        {
            return new VerifyStringListExpression(dataSource);
        }

        /// <summary>
        ///     Verify a list of string values
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static VerifyStringListExpression VerifyStringList(Func<IEnumerable<string>> dataSource)
        {
            return VerifyStringList(c => dataSource());
        }

        /// <summary>
        ///     Use to create a new paragraph grammar to configure an object stored on
        ///     the Context.State.CurrentObject with property
        ///     setter children grammars
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ParagraphGrammar CreateObject<T>(string title, Action<ObjectConstructionExpression<T>> action)
        {
            var grammar = new ParagraphGrammar(title);
            var expression = new ObjectConstructionExpression<T>(grammar);
            action(expression);

            return grammar;
        }

        /// <summary>
        ///     Creates a grammar that will instantiate a new object of type T on the
        ///     Context.State.CurrentObject property and establish child grammar steps
        ///     to set properties on the new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ParagraphGrammar CreateNewObject<T>(string title, Action<ObjectConstructionExpression<T>> action)
            where T : new()
        {
            return CreateObject<T>(title, _ =>
            {
                _.ObjectIs = c => new T();
                action(_);
            });
        }

        /// <summary>
        ///     Creates a grammar that will instantiate a new object of type T on the
        ///     Context.State.CurrentObject property and establish child grammar steps
        ///     to set properties on the new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ParagraphGrammar CreateNewObject<T>(Action<ObjectConstructionExpression<T>> action)
            where T : new()
        {
            return CreateNewObject("Create " + typeof(T).Name, action);
        }

        /// <summary>
        ///     Create a paragraph grammar to verify values on an existing object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ParagraphGrammar VerifyPropertiesOf<T>(string title,
            Action<ObjectVerificationExpression<T>> action)
            where T : class
        {
            var grammar = new ParagraphGrammar(title);

            var builder = new ParagraphBuilder(grammar);

            builder.VerifyPropertiesOf(action);

            return grammar;
        }

        /// <summary>
        ///     Create a generic Paragraph grammar
        /// </summary>
        /// <param name="title"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ParagraphGrammar Paragraph(string title, Action<ParagraphBuilder> configure)
        {
            var grammar = new ParagraphGrammar(title);
            var expression = new ParagraphBuilder(grammar);
            configure(expression);

            return grammar;
        }

        /// <summary>
        ///     Create a set verification grammar against a set of T objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static VerifySetExpression<T> VerifySetOf<T>(Func<IExecutionContext, IEnumerable<T>> dataSource)
        {
            return new VerifySetExpression<T>(dataSource);
        }

        /// <summary>
        ///     Create a set verification grammar against a set of T objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static VerifySetExpression<T> VerifySetOf<T>(Func<IEnumerable<T>> dataSource)
        {
            return new VerifySetExpression<T>(c => dataSource());
        }

        /// <summary>
        ///     A shorthand way of performing simple actions with a single input cell
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ActionGrammar<T> Read<T>(string key, Action<T> action)
        {
            return new ActionGrammar<T>("Read {" + key + "}", (x, context) => action(x));
        }

        /// <summary>
        ///     Imports an existing grammar from another Fixture with options to change
        ///     titles or set different default values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grammarKey"></param>
        /// <returns></returns>
        public ImportedGrammar Import<T>(string grammarKey) where T : Fixture
        {
            var fixture = (T) FixtureLibrary.FixtureCache[typeof(T)];
            return new ImportedGrammar(fixture, fixture[grammarKey]);
        }

        /// <summary>
        ///     Create a different "curried" version of an existing grammar on
        ///     this Fixture with different labels and/or defaults
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CurryGrammarExpression Curry(string key)
        {
            return Curry(this[key]);
        }

        /// <summary>
        ///     Create a different "curried" version of an existing grammar
        ///     with different labels and/or defaults
        /// </summary>
        /// <param name="inner"></param>
        /// <returns></returns>
        public CurryGrammarExpression Curry(IGrammar inner)
        {
            return new CurryGrammarExpression(inner);
        }

        /// <summary>
        ///     Create selection lists used within grammars from this Fixture only
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public void AddSelectionValues(string key, params string[] values)
        {
            Lists[key].AddValues(values);
        }

        /// <summary>
        ///     Gets the values defined for a selection list added using <see cref="AddSelectionValues" />.
        /// </summary>
        /// <param name="key"></param>
        public IEnumerable<string> GetSelectionValues(string key)
        {
            if (Lists.Has(key))
                return Lists[key].Options.Select(x => x.value);

            return Enumerable.Empty<string>();
        }

// SAMPLE: Fixture.Trace
        /// <summary>
        ///     Writes the message to the Storyteller "Debug" results tab
        /// </summary>
        /// <param name="message"></param>
        public void WriteTrace(string message)
        {
#if NET46
            Debug.WriteLine(message);
#else
            Trace.WriteLine(message);
#endif
        }
// ENDSAMPLE

        public class FactExpression
        {
            private readonly string _title;

            public FactExpression(string title)
            {
                _title = title;
            }

            /// <summary>
            ///     Register the Func that can be evaluated as a boolean
            ///     to determine the success or failure of this Fact
            /// </summary>
            /// <param name="test"></param>
            /// <returns></returns>
            public IGrammar VerifiedBy(Func<bool> test)
            {
                return new FactGrammar(_title, c => test());
            }

            /// <summary>
            ///     Register the Func that can be evaluated as a boolean
            ///     to determine the success or failure of this Fact
            /// </summary>
            /// <param name="test"></param>
            /// <returns></returns>
            public IGrammar VerifiedBy(Func<IExecutionContext, bool> test)
            {
                return new FactGrammar(_title, test);
            }

            /// <summary>
            ///     Register the Func that works against
            ///     a service in the TestContext that can be evaluated as a boolean
            ///     to determine the success or failure of this Fact
            /// </summary>
            /// <param name="test"></param>
            /// <returns></returns>
            public IGrammar VerifiedBy<TService>(Func<TService, bool> test)
            {
                return new FactGrammar(_title, c => test(c.Service<TService>()));
            }
        }

        /// <summary>
        /// Shortcut to use an grammar to build a model object with a separate
        /// embedded fixture and assign the model object
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected BuildModelExpression<TModel> Build<TModel>(string title = null) where TModel : class
        {
            return new BuildModelExpression<TModel>(title);
        }

        public class BuildModelExpression<T> where T : class
        {
            private readonly string _title;

            public BuildModelExpression(string title)
            {
                _title = title;
            }

            /// <summary>
            /// Specify which ModelFixture class you want to use to build up
            /// the model of T
            /// </summary>
            /// <typeparam name="TFixture"></typeparam>
            /// <returns></returns>
            public BuildModelWithFixture<T, TFixture> With<TFixture>() where TFixture : ModelFixture<T>, new()
            {
                return new BuildModelWithFixture<T, TFixture>(_title);
            }

            public class BuildModelWithFixture<TModel, TFixture> where TFixture : Fixture, new()
            {
                private readonly EmbeddedSectionGrammar<TFixture> _grammar;

                public BuildModelWithFixture(string title = "If the input is")
                {
                    _grammar = new EmbeddedSectionGrammar<TFixture> {Title = title};
                }

                /// <summary>
                /// Hook to receive the model object built by the embedded section
                /// within the original Fixture
                /// </summary>
                /// <param name="action"></param>
                /// <returns></returns>
                public EmbeddedSectionGrammar<TFixture> Forward(Action<TModel> action)
                {
                    return _grammar.After(c =>
                    {
                        var model = c.State.Retrieve<TModel>();
                        c.State.CurrentObject = model;

                        action(model);
                    });
                }

                /// <summary>
                /// Hook to receive the model object built by the embedded section
                /// within the original Fixture
                /// </summary>
                /// <param name="action"></param>
                /// <returns></returns>
                public EmbeddedSectionGrammar<TFixture> Forward(Func<TModel, Task> action)
                {
                    return _grammar.After(c =>
                    {
                        var model = c.State.Retrieve<TModel>();
                        c.State.CurrentObject = model;

                        return action(model);
                    });
                }
            }
        }
        */
    }
    
}