#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();
extern UIView *UnityGetGLView();

//I also like to include these two convenience methods to convert between c string and NSString*. You need to return a copy of the c string so that Unity handles the memory and gets a valid value.
char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

// This takes a char* you get from Unity and converts it to an NSString* to use in your objective c code. You can mix c++ and objective c all in the same file.
static NSString* CreateNSString(const char* string)
{
    if (string != NULL)
        return [NSString stringWithUTF8String:string];
    else
        return [NSString stringWithUTF8String:""];
}

extern "C"
{
    int _pow2(int x)
    {
        // Just a simple example of returning an int value
        return x * x;
    }
    
    // Returns a char* (a string to Unity)
    char* _helloWorldString()
    {
        // We can use NSString and go to the c string that Unity wants
        NSString *helloString = @"Hello World";
        // UTF8String method gets us a c string. Then we have to malloc a copy to give to Unity. I reuse a method below that makes it easy.
        return cStringCopy([helloString UTF8String]);
    }
    
    // Here is an example of getting a string from Unity
    char* _combineStrings(const char* cString1, const char* cString2)
    {
        // This shows we can create two NSStrings* from the c strings from Unity
        NSString *string1 = CreateNSString(cString1);
        NSString *string2 = CreateNSString(cString2);
        NSString *combinedString = [NSString stringWithFormat:@"%@ %@", string1, string2];
        // Same as before, have to go to a c string and then malloc a copy of it to give to Unity
        return cStringCopy([combinedString UTF8String]);
    }
    
    void _print()
    {
        NSMutableString *printBody = [NSMutableString stringWithFormat:@"\n\nChatpong Suteesuksataporn"];
        //[printBody appendFormat:@"\n\n\n\nPrinted From *myapp*"];
        
        UIPrintInteractionController *pic = [UIPrintInteractionController sharedPrintController];
        //pic.delegate = UnityGetGLViewController();
        
        UIPrintInfo *printInfo = [UIPrintInfo printInfo];
        printInfo.outputType = UIPrintInfoOutputGeneral;
        printInfo.orientation = UIPrintInfoOrientationLandscape;
        //printInfo.jobName = self.titleLabel.text;
        pic.printInfo = printInfo;
        
        UISimpleTextPrintFormatter *textFormatter = [[UISimpleTextPrintFormatter alloc] initWithText:printBody];
        textFormatter.startPage = 0;
        textFormatter.font=[UIFont fontWithName:@"Arail Bold" size:72];
        //textFormatter.contentInsets = UIEdgeInsetsMake(12.0, 12.0, 12.0, 12.0); // 1 inch margins
        //textFormatter.maximumContentWidth = 6 * 72.0;
        textFormatter.textAlignment = NSTextAlignmentCenter;
        pic.printFormatter = textFormatter;
        
        //[textFormatter release];
        //pic.showsPageRange = YES;
        
        void (^completionHandler)(UIPrintInteractionController *, BOOL, NSError *) =
        ^(UIPrintInteractionController *printController, BOOL completed, NSError *error) {
            if (!completed && error) {
                NSLog(@"Printing could not complete because of error: %@", error);
            }
        };
        
        [pic presentFromRect:CGRectMake(0, UnityGetGLView().frame.size.height - 22 * 2, UnityGetGLView().frame.size.width, 22) inView:UnityGetGLView() animated:YES completionHandler:completionHandler];
    }
}