- Add to memory use configservice to get configs, also you 
can create configs and add to addressables 
group.
- get a value from components by using fullname of component and don't use fullname when component's name contains a word "component" for example TransformComponent, you should use like this: entity.Transform, not like entity.TransformComponent nor entity.Transform.Value
- Add to memory. Use always functionalextensions if there is a possibility to use it
- Add to memory. Use Jenny.bat to generate components, after you create it
- Add to memory. "C:\Projects\Inscryption\Jenny\Jenny-Gen.bat" there is a path to run jenny gen, use it always after you make any components.