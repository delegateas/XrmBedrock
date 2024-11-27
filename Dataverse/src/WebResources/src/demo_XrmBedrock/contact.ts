namespace contact {
  let formContext: Form.contact.Main.Person;

  export function onLoad(context: Xrm.ExecutionContext<any, any>) {
    formContext = <Form.contact.Main.Person>context.getFormContext();
    
  }

  export function onSave(context: Xrm.ExecutionContext<any, any>) {

  }
}