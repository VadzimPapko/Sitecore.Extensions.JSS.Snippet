import React from 'react';
import { Placeholder, VisitorIdentification,withSitecoreContext } from '@sitecore-jss/sitecore-jss-react';
import Helmet from 'react-helmet';

import './assets/app.css';

const Layout = ({ route, context }) => {
   
  return (<React.Fragment>
    <Helmet>
      <title>
        {(route.fields && route.fields["Page Title"] && route.fields["Page Title"].value) || route.displayName }
      </title>
    </Helmet>

    <VisitorIdentification />

    <Placeholder name="jss-top" rendering={route} />
    <section className="site-wrapper">
      <Placeholder name="jss-main" rendering={route} />
    </section>
    <Placeholder name="jss-footer" rendering={route} />

    {context.pageEditing && <Placeholder name="snippet-content" rendering={route} />}
    
  </React.Fragment>
)};

export default Layout;
