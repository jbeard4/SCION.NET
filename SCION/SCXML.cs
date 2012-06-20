using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using org.mozilla.javascript;

namespace SCION
{
    //this wraps the java list interface
    internal class JavaListAdapter<T> : IList<T> {
        private java.util.List list;

        public JavaListAdapter(java.util.List list){
            this.list = list;
        }

        public void Add(T s){
            this.list.add(s);
        }

        public void Clear(){
            this.list.clear();
        }

        public bool Contains(T o){
            return this.list.contains(o);
        }

        public void CopyTo(T[] array,int index){
            int j = index;
            for (int i = 0; i < Count; i++)
            {
                array.SetValue(this.list.get(i), j);
                j++;
            }
        }

        public IEnumerator<T> GetEnumerator(){
            java.util.Iterator iterator = this.list.iterator();
            while(iterator.hasNext()){
                yield return (T) iterator.next();
            }
        }

        IEnumerator IEnumerable.GetEnumerator(){
            java.util.Iterator iterator = this.list.iterator();
            while(iterator.hasNext()){
                yield return iterator.next();
            }
        }

        public int IndexOf(T value){
            return this.list.indexOf(value);
        }

        public void Insert(int index, T value){
            this.list.set(index,value);
        }

        public bool Remove(T value){
            return this.list.remove(value);
        }

        public void RemoveAt(int index){
            this.list.remove(index);
        }

        public int Count
        {
            get
            {
                return this.list.size();
            }
        }

        public T this[int index]
        {
            get
            {
                return (T) this.list.get(index);
            }
            set
            {
                this.list.set(index,value);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

    }

    public class SCXML{

        private static readonly com.inficon.scion.SCION scion = new com.inficon.scion.SCION();
        private Scriptable interpreter;

        /**
        * Accepts a path to an SCXML file and returns a Scriptable model object, which can be passed 
        * to the SCXML constructor.
        */
        public static Model PathToModel(string path){
            return new Model((Scriptable) SCXML.scion.pathToModel(path));
        }

        /**
        * Accepts an org.w3c.dom.Document object and returns a Scriptable model object, which can be passed 
        * to the SCXML constructor.
        */
        public static Model DocumentToModel(XmlDocument doc){
            return new Model((Scriptable) SCXML.scion.documentStringToModel(doc.DocumentElement.OuterXml));
        }

        /**
        * Accepts an String containing an unparsed XML document and returns a Scriptable model object,
        * which can be passed to the SCXML constructor.
        */
        public static Model DocumentStringToModel(string docString){
            return new Model((Scriptable) SCXML.scion.documentStringToModel(docString));
        }

        //constructor
        public SCXML(Model model){
            this.interpreter = (Scriptable) scion.createScionInterpreter(model.getModel());
        }   

        public SCXML(string path){
            Model m = SCXML.PathToModel(path);
            this.interpreter = (Scriptable) scion.createScionInterpreter(m.getModel());
        }   

        public SCXML(XmlDocument doc){
            Model m = SCXML.DocumentToModel(doc);
            this.interpreter = (Scriptable) scion.createScionInterpreter(m.getModel());
        }   

        //starts the interpreter, returns a string list of state names
        //TODO: should make him a Set<String>
        /**
        * Starts the interpreter; should  be called before the first time gen is called, and 
        * should only be called once in an SCXML object's lifespan.
        */
        public IList<string> Start(){
            //FIXME: do we need to convert js array to C# array in order to cast him to the C# list interface?
            //or will this play nice?
            java.util.List configuration = (java.util.List) scion.startInterpreter(this.interpreter);
            IList<string> conf = new JavaListAdapter<string>(configuration);
            return conf;
        }

        /**
        * Sends an event to the interpreter, which will prompt the interpreter
        * to take a macrostep as described in <a href="https://github.com/jbeard4/SCION/wiki/Scion-Semantics">SCION Semantics</a>.
        */
        public IList<string> Gen(string eventName, object eventData){
            return (IList<string>) new JavaListAdapter<string>((java.util.List) scion.genEvent(this.interpreter,eventName,eventData));
        }

    }

    public class Model{
        private Scriptable scriptable;

        public Model(Scriptable scriptable){
            this.scriptable = scriptable;
        }

        public Scriptable getModel(){
            return this.scriptable;
        }
    }
}
