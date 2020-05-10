using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Heartland.contracts;
using Heartland.models;
using Heartland.repositories;
using Heartland.util;

namespace Heartland{
    public class ContactsListApp{
        private  IContactRepository _ContactRepository;
        private int? _PreviousCount;
        private  int? _Count;

        public ContactsListApp(IContactRepository contactRepository)
        {
            // Initialize
            _ContactRepository = contactRepository;
            _ContactRepository.StartListeningForUpdateCount();

            // TODO: For future Realtime update to count
            _ContactRepository.UpdateCountEvent += UpdateCount;
        }
        public async Task Run()
        {
            // Setup Console
            Console.Clear();
            Console.WriteLine();

            Contact _contact = new Contact();
            do{
                CustomConsole.ClearConsole();
                await CustomConsole.UpdateCount(_ContactRepository);
                foreach(var prop in typeof(Contact).GetProperties()){
                    bool _IsValidProp = true;
                    string _optionalString = "";

                    // Get Data Annotations
                    var _name = prop.GetCustomAttribute<DisplayAttribute>().Name;
                    var _optional = prop.GetCustomAttribute<RequiredAttribute>();
                    var _phone = prop.GetCustomAttribute<PhoneAttribute>();
                    var _customAttributes = prop.GetCustomAttributes();
                    do{
                        _IsValidProp = true;
                        // Pompt user
                        
                        _optionalString = _optional == null? "[OPTIONAL]" : "";
                        Console.WriteLine($"Please enter a value for the contact's {_name} {_optionalString}");

                        // Get user input
                        var _value = Console.ReadLine();

                        // Validate
                        foreach(var attr in _customAttributes){
                            switch(attr.GetType().Name){
                                case "RequiredAttribute":
                                    if(!_optional.IsValid(_value)){
                                        _IsValidProp = false;
                                        Console.WriteLine($"{_name} is a required field");
                                    }
                                    break;
                                case "PhoneAttribute":
                                    if(!_phone.IsValid(_value)){
                                        _IsValidProp = false;
                                        Console.WriteLine($"{_name} is requires a valid telephone number format");
                                    }
                                    break;
                            }
                        }
                            
                        if(_IsValidProp)
                            prop.SetValue(_contact, _value);
                        
                    }while(!_IsValidProp);

                    CustomConsole.ClearConsole();
                }

                await _ContactRepository.Add(_contact);
                Console.WriteLine("Enter {ESC} quit, {ANY OTHER KEY} add new contact");
            } while(Console.ReadKey().Key != ConsoleKey.Escape);
            _ContactRepository.Dispose();
        }

        private void UpdateCount(Object sender, EventArgs args ){
            CustomConsole.UpdateCount(_ContactRepository).Wait();
        }
    }
}