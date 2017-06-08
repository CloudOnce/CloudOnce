//
//  CloudOnceCloudSave.mm
//  CloudOnceCloudSave
//
//  Created by Jan Ivar Z. Carlsen on 21/04/15.
//  Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#import "CloudOnceCloudSave.h"

@implementation CloudOnceCloudSave

static char* const CallbackObject = (char*)"CloudOnceCallbackObject";
static char* const CallbackMethod = (char*)"ExternalChange";

-(id)init
{
    NSLog(@"CloudOnceCloudSave init");
    self = [super init];
    if ([NSUbiquitousKeyValueStore defaultStore])
    {
        NSUbiquitousKeyValueStore* iCloudKVStore = [NSUbiquitousKeyValueStore defaultStore];
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(updateFromiCloud:)
                                                     name:NSUbiquitousKeyValueStoreDidChangeExternallyNotification
                                                   object:iCloudKVStore];
        [iCloudKVStore synchronize];
    }
    return self;
}

- (BOOL) setDevString: (NSString*) key : (NSString*) value
{
    if ([NSUbiquitousKeyValueStore defaultStore])
    {
        NSUbiquitousKeyValueStore* iCloudKVStore = [NSUbiquitousKeyValueStore defaultStore];
        [iCloudKVStore setString:value forKey:key];
        return true;
    }
    else
    {
        return false;
    }
}

- (const char*) getDevString: (NSString*) key
{
    if ([NSUbiquitousKeyValueStore defaultStore])
    {
        NSUbiquitousKeyValueStore* iCloudKVStore = [NSUbiquitousKeyValueStore defaultStore];
        NSLog(@"Getting dev string %@", key);
        return [self makeStringCopy:[[iCloudKVStore stringForKey:key] UTF8String]];
    }
    else
    {
        return (char*)"";
    }
}

- (BOOL) deleteDevString: (NSString*) key
{
    if ([NSUbiquitousKeyValueStore defaultStore])
    {
        NSUbiquitousKeyValueStore* iCloudKVStore = [NSUbiquitousKeyValueStore defaultStore];
        [iCloudKVStore removeObjectForKey:key];
        return true;
    }
    else
    {
        return false;
    }
}

- (void) updateFromiCloud:(NSNotification*) notification
{
    // Get the list of keys that changed.
    NSDictionary* userInfo = [notification userInfo];
    NSNumber* reasonForChange = [userInfo objectForKey:NSUbiquitousKeyValueStoreChangeReasonKey];
    NSInteger reason = -1;
    
    // If a reason cloud not be determined, do not update anything.
    if (!reasonForChange) {
        return;
    }
    
    reason = [reasonForChange integerValue];
    NSArray* changedKeys = [userInfo objectForKey:NSUbiquitousKeyValueStoreChangedKeysKey];
    NSUbiquitousKeyValueStore* iCloudKVStore = [NSUbiquitousKeyValueStore defaultStore];
    NSMutableString* messageString = [NSMutableString stringWithFormat:@"%ld", (long)reason];
    for (NSString* key in changedKeys) {
        id value = [iCloudKVStore objectForKey:key];
        if ([value isKindOfClass:[NSString class]]) {
            [messageString appendFormat:@"|%@.%@", key, value];
        }
    }
    
    UnitySendMessage([self makeStringCopy:CallbackObject], [self makeStringCopy:CallbackMethod], [self makeStringCopy:[messageString UTF8String]]);
}

- (char *) makeStringCopy: (const char *) string
{
    if (string)
    {
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    else
    {
        return (char*)"";
    }
}

// Converts C style string to NSString
- (NSString *) createNSString: (const char *) string
{
    if(string)
    {
        return [NSString stringWithUTF8String: string];
    }
    else
    {
        return [NSString stringWithUTF8String:""];
    }
}

@end

static CloudOnceCloudSave* cloudSaveObject = nil;

void EnsureCloudSaveObjectIsNotNil()
{
    if (cloudSaveObject == nil) {
        cloudSaveObject = [[CloudOnceCloudSave alloc] init];
    }
}

extern "C" {
    
    BOOL _SetDevString(const char* key, const char* value)
    {
        NSLog(@"Set string with key: %s value: %s", key, value);
        EnsureCloudSaveObjectIsNotNil();
        return [cloudSaveObject setDevString:[cloudSaveObject createNSString:key] :[cloudSaveObject createNSString:value]];
    }
    
    const char* _GetDevString(const char* key)
    {
        NSLog(@"Get string with key: %s", key);
        EnsureCloudSaveObjectIsNotNil();
        const char* returnString = [cloudSaveObject getDevString:[cloudSaveObject createNSString:key]];
        NSLog(@"Returning string: %s", returnString);
        return [cloudSaveObject makeStringCopy:returnString];
    }
    
    BOOL _DeleteDevString(const char* key)
    {
        NSLog(@"Delete string with key: %s", key);
        EnsureCloudSaveObjectIsNotNil();
        return [cloudSaveObject deleteDevString:[cloudSaveObject createNSString:key]];
    }
    
}
