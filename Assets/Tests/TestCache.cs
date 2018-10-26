using System;
using NUnit.Framework;
using Neo.Collections;
using Neo.Async;

namespace Tests.Neo.Async{
  [TestFixture]
  public sealed class TestCache{
    private sealed class FakeRepo{
      public readonly Dictionary<string,List<Action<string>>> Callbacks = new Dictionary<string,List<Action<string>>>();

      public void Get(string key, Action<string> callback){
        if(!Callbacks.Has(key)) Callbacks[key] = new List<Action<string>>();
        Callbacks[key].Add(callback);
      }

      public void Perform(string key, string val){
        Callbacks[key].ForEach( callback => callback(val) );
        Callbacks[key].Clear();
      }
    }

    private FakeRepo repo;
    private Cache<string, string> subject;

    [SetUp]
    public void SetUp(){
      repo = new FakeRepo();
      subject = new Cache<string, string>(repo.Get);
    }

    [Test]
    public void GetUsesAQueue(){
      string gotA1 = null;
      string gotA2 = null;
      string gotB1 = null;
      string gotB2 = null;

      subject.Get("A", got => gotA1 = got);

      Assert.AreEqual(1, repo.Callbacks.Count);
      Assert.AreEqual(1, repo.Callbacks["A"].Count);
      Assert.IsNull(gotA1);
      Assert.IsNull(gotA2);

      subject.Get("A", got => gotA2 = got);
      Assert.AreEqual(1, repo.Callbacks.Count);
      Assert.AreEqual(1, repo.Callbacks["A"].Count);

      Assert.IsNull(gotA1);
      Assert.IsNull(gotA2);

      string foo = "foo";
      repo.Perform("A", foo);
      Assert.AreSame(foo, gotA1);
      Assert.AreSame(foo, gotA2);

      Assert.AreEqual(1, repo.Callbacks.Count);
      Assert.AreEqual(0, repo.Callbacks["A"].Count);

      //ask for next one

      subject.Get("B", got => gotB1 = got);

      Assert.AreEqual(2, repo.Callbacks.Count);
      Assert.AreEqual(0, repo.Callbacks["A"].Count);
      Assert.AreEqual(1, repo.Callbacks["B"].Count);
      Assert.AreSame(foo, gotA1);
      Assert.AreSame(foo, gotA2);
      Assert.IsNull(gotB1);
      Assert.IsNull(gotB2);

      subject.Get("B", got => gotB2 = got);
      Assert.AreEqual(2, repo.Callbacks.Count);
      Assert.AreEqual(0, repo.Callbacks["A"].Count);
      Assert.AreEqual(1, repo.Callbacks["B"].Count);
      Assert.AreSame(foo, gotA1);
      Assert.AreSame(foo, gotA2);
      Assert.IsNull(gotB1);
      Assert.IsNull(gotB2);

      string bar = "bar";
      repo.Perform("B", bar);
      Assert.AreSame(foo, gotA1);
      Assert.AreSame(foo, gotA2);
      Assert.AreSame(bar, gotB1);
      Assert.AreSame(bar, gotB2);

      Assert.AreEqual(2, repo.Callbacks.Count);
      Assert.AreEqual(0, repo.Callbacks["A"].Count);
      Assert.AreEqual(0, repo.Callbacks["B"].Count);

      //Ask again for A
      string gotA3 = null;
      subject.Get("A", got => gotA3 = got);

      Assert.AreEqual(2, repo.Callbacks.Count);
      Assert.AreEqual(0, repo.Callbacks["A"].Count);
      Assert.AreEqual(0, repo.Callbacks["B"].Count);

      Assert.AreSame(foo, gotA1);
      Assert.AreSame(foo, gotA2);
      Assert.AreSame(foo, gotA3);
      Assert.AreSame(bar, gotB1);
      Assert.AreSame(bar, gotB2);
    }

    [Test]
    public void Clear(){
      string gotA1 = null;

      subject.Get("A", got => gotA1 = got);

      Assert.AreEqual(1, repo.Callbacks.Count);
      Assert.AreEqual(1, repo.Callbacks["A"].Count);

      string foo = "foo";
      repo.Perform("A", foo);
      Assert.AreSame(foo, gotA1);

      Assert.AreEqual(1, repo.Callbacks.Count);
      Assert.AreEqual(0, repo.Callbacks["A"].Count);

      subject.Clear();
      subject.Get("A", got => gotA1 = got);

      Assert.AreEqual(1, repo.Callbacks.Count);
      Assert.AreEqual(1, repo.Callbacks["A"].Count);

      string bar = "bar";
      repo.Perform("A", bar);
      Assert.AreSame(bar, gotA1);
    }
  }
}
