module Quotes

open System

let randomQuotes = [|
    "Every strike brings me closer to the next home run. – Babe Ruth";
    "I don’t walk away from things I think are unfinished. – Arnold Schwarzenegger";
    "Whether you think you can or you think you can’t, you’re right. –Henry Ford";
    "I’ve missed more than 9000 shots in my career. I’ve lost almost 300 games. 26 times I’ve been trusted to take the game winning shot and missed. I’ve failed over and over and over"; "again in my life. And that is why I succeed. –Michael Jordan";
    "I didn’t fail the test. I just found 100 ways to do it wrong. –Benjamin Franklin";
    "A person who never made a mistake never tried anything new. – Albert Einstein";
    "It is never too late to be what you might have been. –George Eliot";
    "Twenty years from now you will be more disappointed by the things that you didn’t do than by the ones you did do, so throw off the bowlines, sail away from safe harbor, catch the"; "trade winds in your sails.  Explore, Dream, Discover. –Mark Twain";
    "The most common way people give up their power is by thinking they don’t have any. –Alice Walker";
    "The mind is everything. What you think you become.  –Buddha";
    "Either you run the day, or the day runs you. –Jim Rohn";
    "Life shrinks or expands in proportion to one’s courage. –Anais Nin";
    "You can’t fall if you don’t climb.  But there’s no joy in living your whole life on the ground. –Unknown";
    "We must believe that we are gifted for something, and that this thing, at whatever cost, must be attained. –Marie Curie";
    "Too many of us are not living our dreams because we are living our fears. –Les Brown";
    "Challenges are what make life interesting and overcoming them is what makes life meaningful. –Joshua J. Marine";
    "If you want to lift yourself up, lift up someone else. –Booker T. Washington";
    "I have been impressed with the urgency of doing. Knowing is not enough; we must apply. Being willing is not enough; we must do. –Leonardo da Vinci";
    "Limitations live only in our minds.  But if we use our imaginations, our possibilities become limitless. –Jamie Paolinetti";
    "You become what you believe. –Oprah Winfrey";
    "A truly rich man is one whose children run into his arms when his hands are empty. –Unknown";
    "The two most important days in your life are the day you are born and the day you find out why. – Mark Twain";
    "You miss 100% of the shots you don’t take. –Wayne Gretzky";
    "People often say that motivation doesn’t last. Well, neither does bathing.  That’s why we recommend it daily. –Zig Ziglar";
    "There is only one way to avoid criticism: do nothing, say nothing, and be nothing. –Aristotle";
    "Believe you can and you’re halfway there. –Theodore Roosevelt";
    "Everything you’ve ever wanted is on the other side of fear. –George Addair";
    "Definiteness of purpose is the starting point of all achievement. –W. Clement Stone";
    "If you hear a voice within you say “you cannot paint,” then by all means paint and that voice will be silenced. –Vincent Van Gogh";
    "Ask and it will be given to you; search, and you will find; knock and the door will be opened for you. –Jesus";
    "If the wind will not serve, take to the oars. –Latin Proverb";
    "Dream big and dare to fail. –Norman Vaughan";
    "Our lives begin to end the day we become silent about things that matter. –Martin Luther King Jr.";
    "You may be disappointed if you fail, but you are doomed if you don’t try. –Beverly Sills";
    "Two roads diverged in a wood, and I—I took the one less traveled by, And that has made all the difference.  –Robert Frost";
    "Whatever you can do, or dream you can, begin it.  Boldness has genius, power and magic in it. –Johann Wolfgang von Goethe";
    "Every child is an artist.  The problem is how to remain an artist once he grows up. –Pablo Picasso";
    "We can easily forgive a child who is afraid of the dark; the real tragedy of life is when men are afraid of the light. –Plato";
    "Life is 10% what happens to me and 90% of how I react to it. –Charles Swindoll";
    "An unexamined life is not worth living. –Socrates";
    "Your time is limited, so don’t waste it living someone else’s life. –Steve Jobs";
    "I’ve learned that people will forget what you said, people will forget what you did, but people will never forget how you made them feel. –Maya Angelou";
    "Few things can help an individual more than to place responsibility on him, and to let him know that you trust him.  –Booker T. Washington";
    "Certain things catch your eye, but pursue only those that capture the heart. – Ancient Indian Proverb";
    "Start where you are. Use what you have.  Do what you can. –Arthur Ashe";
    "When I stand before God at the end of my life, I would hope that I would not have a single bit of talent left and could say, I used everything you gave me. –Erma Bombeck";
    "How wonderful it is that nobody need wait a single moment before starting to improve the world. –Anne Frank";
    "When I let go of what I am, I become what I might be. –Lao Tzu";
    "Life is not measured by the number of breaths we take, but by the moments that take our breath away. –Maya Angelou";
    "Happiness is not something readymade.  It comes from your own actions. –Dalai Lama";
    "The best way to get accurate information on Usenet is to post something wrong and wait for corrections.";
    "The computer was born to solve problems that did not exist before.";
    "Q: Is the glass half-full or half-empty? A: The glass is twice as big as it needs to be.";
    "In theory, there ought to be no difference between theory and practice. In practice, there is.";
    "There is no Ctrl-Z in life.";
    "Whitespace is never white.";
    "When all else fails … reboot.";
|]

let randomHashtags = [|
    "#love1";
    "#instagood";
    "#fashion";
    "#photooftheday";
    "#beautiful";
    "#art";
    "#photography";
    "#happy";
    "#picoftheday";
    "#cute";
    "#follow";
    "#tbt";
    "#followme";
    "#nature";
    "#like4like";
    "#travel";
    "#instagram";
    "#style";
    "#repost";
    "#summer";
    "#instadaily";
    "#selfie";
    "#me";
    "#friends";
    "#fitness";
    "#girl";
    "#food";
    "#fun";
    "#beauty";
    "#instalike";
    "#smile";
    "#family";
    "#photo";
    "#life";
    "#likeforlike";
    "#music";
    "#ootd";
    "#follow4follow";
    "#makeup";
    "#amazing";
    "#igers";
    "#nofilter";
    "#dog";
    "#model";
    "#sunset";
    "#beach";
    "#instamood";
    "#foodporn";
    "#motivation";
    "#followforfollow";
|]


let randomReplies = [|
    "Any fool can write code that a computer can understand. Good programmers write code that humans can understand.";
    "First, solve the problem. Then, write the code.";
    "Experience is the name everyone gives to their mistakes.";
    " In order to be irreplaceable, one must always be different";
    "Java is to JavaScript what car is to Carpet.";
    "Knowledge is power.";
    "Sometimes it pays to stay in bed on Monday, rather than spending the rest of the week debugging Monday’s code.";
    "Perfection is achieved not when there is nothing more to add, but rather when there is nothing more to take away.";
    "Ruby is rubbish! PHP is phpantastic!";
    " Code is like humor. When you have to explain it, it’s bad.";
    "Fix the cause, not the symptom.";
    "Optimism is an occupational hazard of programming: feedback is the treatment";
    "When to use iterative development? You should use iterative development only on projects that you want to succeed.";
    "Simplicity is the soul of efficiency.";
    "Before software can be reusable it first has to be usable.";
    "Make it work, make it right, make it fast.";
    "The best thing about a boolean is even if you are wrong, you are only off by a bit.";
    "The best method for accelerating a computer is the one that boosts it by 9.8 m/s2.";
    "There are two ways to write error-free programs; only the third one works.";
    "It’s not a bug – it’s an undocumented feature.";
    "A good programmer is someone who always looks both ways before crossing a one-way street.";
    "Software undergoes beta testing shortly before it’s released. Beta is Latin for “still doesn’t work.";
    "Every big computing disaster has come from taking too many ideas and putting them in one place.";
    "Always code as if the person who ends up maintaining your code will be a violent psychopath who knows where you live.";
    "Debugging is twice as hard as writing the code in the first place. Therefore, if you write the code as cleverly as possible, you are, by definition, not smart enough to debug it.";
    "A good way to stay flexible is to write less code.";
    "Don’t worry if it doesn’t work right. If everything did, you’d be out of a job.";
    "The trouble with programmers is that you can never tell what a programmer is doing until it’s too late.";
    "In order to understand recursion, one must first understand recursion.";
    "Software undergoes beta testing shortly before it’s released. Beta is Latin for “still doesn’t work.";
    "If debugging is the process of removing software bugs, then programming must be the process of putting them in";
    "Walking on water and developing software from a specification are easy if both are frozen.";
|]


let getRandomQuote() = 
    randomQuotes.[(Random().Next(0, randomQuotes.Length))]


let getRandomHashtag() =
    randomHashtags.[(Random().Next(0, randomHashtags.Length))]


let getRandomReply() = 
    randomReplies.[(Random().Next(0, randomReplies.Length))]