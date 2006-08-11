namespace NxBRE.InferenceEngine.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	
	using NxBRE.InferenceEngine.Rules;
	
	using NxBRE.Util;
	
	internal abstract class FactEnumeratorFactory
	{
		private FactEnumeratorFactory() {}
		
		public static IEnumerator<Fact> NewSingleFactEnumerator(Fact fact) {
			if (Logger.IsInferenceEngineVerbose) Logger.InferenceEngineSource.TraceEvent(TraceEventType.Verbose, 0, "NewSingleFactEnumerator: " + fact);
			return new SingleFactEnumerator(fact);
		}
		
		public static IEnumerator<Fact> NewFactListExcludingEnumerator(IList<Fact> factList, IList<Fact> excludedFacts) {
			if (Logger.IsInferenceEngineVerbose) Logger.InferenceEngineSource.TraceEvent(TraceEventType.Verbose, 0, "NewFactListExcludingEnumerator: factList.Count=" + factList.Count + " - excludedFacts.Count=" + excludedFacts.Count);
			return new FactListEnumerator(factList, excludedFacts);
		}
		
		public static IEnumerator<Fact> NewFactListPredicateMatchingEnumerator(IList<Fact> factList, Atom filter, bool strictTyping, IList<int> ignoredPredicates, IList<Fact> excludedFacts) {
			if (Logger.IsInferenceEngineVerbose) Logger.InferenceEngineSource.TraceEvent(TraceEventType.Verbose, 0, "NewFactListPredicateMatchingEnumerator: factList.Count=" + factList.Count + " - filter=" + filter + " - strictTyping=" + strictTyping + " - ignoredPredicates=" + Misc.IListToString((System.Collections.IList)ignoredPredicates) + " - excludedFacts.Count=" + excludedFacts.Count);
			return new FactListPredicateMatchingEnumerator(factList, filter, strictTyping, ignoredPredicates, excludedFacts);
		}
			
		// ---- Private Classes -----
		
		private abstract class AbstractExcludingFactEnumerator<Fact> : IEnumerator<Fact> {
			private const int RESET = -1;
			
			protected IList<Fact> excludedFacts;
			
			protected int position;
			
			protected Fact currentFact;
			
			protected bool disposed = false;
			
			public AbstractExcludingFactEnumerator():this(null) {}
			
			public AbstractExcludingFactEnumerator(IList<Fact> excludedFacts) {
				this.excludedFacts = excludedFacts;
				Reset();
			}
			
			public virtual void Dispose()
			{
				disposed = true;
				excludedFacts = null;
				currentFact = default(Fact);
			}
			
			private void CheckDisposed() {
				if (disposed) throw new InvalidOperationException("Impossible operation: the fact enumerator has been disposed.");
			}
			
			public void Reset() {
				CheckDisposed();
				position = RESET;
			}
			
			private Fact GetCurrent() {
				CheckDisposed();
				if (position == RESET) throw new InvalidOperationException("Impossible operation: the fact enumerator is not positioned for reading.");
				return currentFact;
			}
			
			object System.Collections.IEnumerator.Current {
				get {
					return GetCurrent();
				}
			}
			
			Fact IEnumerator<Fact>.Current {
				get {
					return GetCurrent();
				}
			}
			
			public virtual bool MoveNext() {
				CheckDisposed();
				
				position++;
				
				if (!DoMoveNext()) return false;
				
				if (currentFact == null) return false;
				
				// if the current fact is in the excluded list, automatically move to the next record
				if ((excludedFacts != null) && (excludedFacts.Contains(currentFact))) return MoveNext();
				else return true;
			}
			
			protected abstract bool DoMoveNext();
		}
	
		private class SingleFactEnumerator : AbstractExcludingFactEnumerator<Fact> {
			private readonly Fact singleFactResult;
			
			public SingleFactEnumerator(Fact singleFactResult):base() {
				this.singleFactResult = singleFactResult;
			}
			
			protected override bool DoMoveNext() {
				if (position > 0) return false;
				currentFact = singleFactResult;
				return true;
			}
	}
	
		private class FactListEnumerator : AbstractExcludingFactEnumerator<Fact> {
			private IList<Fact> factList;
			
			public FactListEnumerator(IList<Fact> factList):this(factList, null) {}
			
			public FactListEnumerator(IList<Fact> factList, IList<Fact> excludedFacts):base(excludedFacts) {
				this.factList = factList;
			}
			
			public override void Dispose() {
				base.Dispose();
				factList = null;
			}
			
			protected override bool DoMoveNext() {
				if (position >= factList.Count) return false;
				currentFact = factList[position];
				return true;
			}
		}
	
		private class FactListPredicateMatchingEnumerator : FactListEnumerator {
			private Atom filter;
			private bool strictTyping;
			private IList<int> ignoredPredicates;
			
			public FactListPredicateMatchingEnumerator(IList<Fact> factList, Atom filter, bool strictTyping, IList<int> ignoredPredicates):this(factList, filter, strictTyping, ignoredPredicates, null) {}
			
			public FactListPredicateMatchingEnumerator(IList<Fact> factList, Atom filter, bool strictTyping, IList<int> ignoredPredicates, IList<Fact> excludedFacts):base(factList, excludedFacts) {
				this.filter = filter;
				this.strictTyping = strictTyping;
				this.ignoredPredicates = ignoredPredicates;
			}
			
			public override void Dispose() {
				base.Dispose();
				filter = null;
				ignoredPredicates = null;
			}
			
			public override bool MoveNext() {
				if (base.MoveNext()) {
					if (Logger.IsInferenceEngineVerbose) Logger.InferenceEngineSource.TraceEvent(TraceEventType.Verbose, 0, "FactListPredicateMatchingEnumerator.MoveNext: currentFact=" + currentFact + " -> " + currentFact.PredicatesMatch(filter, strictTyping, ignoredPredicates));
					
					if (!currentFact.PredicatesMatch(filter, strictTyping, ignoredPredicates)) return MoveNext();
					else return true;
				}
				else {
					return false;
				}
			}
			
		}

	}
}
