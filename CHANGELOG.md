# Changelog
All notable changes to this project will be documented in this file.

## [1.0.79] - 2018-10-07
### Added
- Convenience class for creating partial attributes with values (LdapPartialAttribute)


## [1.0.79] - 2018-10-04
### Enhancements
- Some minor performance improvements when parsing and building attributes


## [1.0.75] - 2018-10-02
### Fixes
- Fix bug when parsing attributes with long notation length


## [1.0.71] - 2018-09-28
### Fixes
- Fix bug when creating attributes with length in long notation [`bf2fd05`](https://github.com/vforteli/Flexinets.Ldap.Core/commit/bf2fd05). Thanks to ([Sammuel-Miranda](https://github.com/Sammuel-Miranda)) for pointing this out.


## [1.0.64] - 2018-06-12
### Fixes
- Make sure attribute.IsContructed returns true also for newly created attributes which dont have the tag bit set [`dc1dbae`](https://github.com/vforteli/Flexinets.Ldap.Core/commit/dc1dbae) 
